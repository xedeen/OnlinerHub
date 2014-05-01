using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using NewsHub.Resources;

namespace NewsHub.ViewModels
{
    public class MainViewModel : LoadViewModelBase
    {
        private readonly object _readLock = new object();

        public MainViewModel()
        {
            IsLoading = false;
            this.Tech = new ObservableCollection<FeedItemViewModel>();
            this.Auto = new ObservableCollection<FeedItemViewModel>();
            this.People = new ObservableCollection<FeedItemViewModel>();
            this.Realt = new ObservableCollection<FeedItemViewModel>();

        }

        public ObservableCollection<FeedItemViewModel> Tech { get; private set; }
        public ObservableCollection<FeedItemViewModel> Auto { get; private set; }
        public ObservableCollection<FeedItemViewModel> People { get; private set; }
        public ObservableCollection<FeedItemViewModel> Realt { get; private set; }

        private bool _storageFeedsRequested = false;
        private FeedType _currentFeedType = FeedType.Auto;
        public FeedType CurrentFeedType
        {
            get { return _currentFeedType; }
            set
            {
                _currentFeedType = value;
                NotifyPropertyChanged("CurrentFeedType");
            }
        }


        
        public readonly AppSettings _appSettings = new AppSettings();
        public AppSettings Settings
        {
            get { return _appSettings; }
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData(FeedType feedType)
        {
            IsLoading = true;
            var wc = new WebClient();
            wc.OpenReadCompleted += FeedReadCompleted;
            _currentFeedType = feedType;

            LoadFeedsFromStorage();

            Uri feedUri;
            switch (feedType)
            {
                case FeedType.Auto:
                    feedUri = new Uri("http://auto.onliner.by/feed");
                    break;
                case FeedType.People:
                    feedUri = new Uri("http://people.onliner.by/feed");
                    break;
                case FeedType.Realt:
                    feedUri = new Uri("http://realt.onliner.by/feed");
                    break;
                default:
                    feedUri = new Uri("http://tech.onliner.by/feed");
                    break;
            }
            wc.OpenReadAsync(feedUri);

        }

        private void FeedReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    //Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(AppResources.FeedUpdateError));
                    return;
                }
                var atLeastOneItemMerged = false;
                using (var reader = XmlReader.Create(e.Result))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    foreach (SyndicationItem item in feed.Items)
                    {
                        SyndicationElementExtension media =
                            item.ElementExtensions.FirstOrDefault(ee => ee.OuterName == "thumbnail");
                        XAttribute attribute = media == null ? null : media.GetObject<XElement>().Attribute("url");

                        var merged = MergeFeedItem(new FeedItemViewModel
                        {
                            Description = item.Summary.Text,
                            Uri = item.Links[0].Uri.ToString(),
                            Title = item.Title.Text,
                            ImageUri = (null != attribute) ? attribute.Value : string.Empty,
                            FeedType = CurrentFeedType,
                            PublishDate = item.PublishDate.LocalDateTime
                        });

                        atLeastOneItemMerged = atLeastOneItemMerged || merged;

                    }
                }
                if (atLeastOneItemMerged)
                    Sort();
            }
            catch (Exception exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(exception.Message));
            }
            finally
            {
                //CurrentFeedType = _currentSelectedPageType;
                IsLoading = false;
            }

        }

        private bool MergeFeedItem(FeedItemViewModel feedItem)
        {
            switch (feedItem.FeedType)
            {
                case FeedType.Auto:
                    if (Auto.All(item => item.Uri != feedItem.Uri))
                    {
                        Auto.Add(feedItem);
                        return true;
                    }
                    break;
                case FeedType.People:
                    if (People.All(item => item.Uri != feedItem.Uri))
                    {
                        People.Add(feedItem);
                        return true;
                    }
                    break;
                case FeedType.Realt:
                    if (Realt.All(item => item.Uri != feedItem.Uri))
                    {
                        Realt.Add(feedItem);
                        return true;
                    }
                    break;
                case FeedType.Tech:
                    if (Tech.All(item => item.Uri != feedItem.Uri))
                    {
                        Tech.Add(feedItem);
                        return true;
                    }
                    break;
            }
            return false;
        }


        private void LoadFeedsFromStorage()
        {
            if (_storageFeedsRequested)
                return;
            try
            {
                lock (_readLock)
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!store.DirectoryExists("Feeds"))
                            store.CreateDirectory("Feeds");

                        foreach (var filePath in store.GetFileNames(Path.Combine("Feeds", "*.feed")))
                        {
                            var feedItem = Load<FeedItemViewModel>(store, Path.Combine("Feeds", filePath));
                            switch (feedItem.FeedType)
                            {
                                case FeedType.Auto:
                                    Auto.Add(feedItem);
                                    break;
                                case FeedType.People:
                                    People.Add(feedItem);
                                    break;
                                case FeedType.Realt:
                                    Realt.Add(feedItem);
                                    break;
                                case FeedType.Tech:
                                    Tech.Add(feedItem);
                                    break;
                            }
                        }
                    }
                }
            }
            finally
            {
                Sort(true);
                _storageFeedsRequested = true;
            }
        }

        private void Sort(bool sortAll = false)
        {
            if (!sortAll)
                Sort(_currentFeedType);
            else
            {
                Sort(FeedType.Auto);
                Sort(FeedType.People);
                Sort(FeedType.Realt);
                Sort(FeedType.Tech);
            }
        }

        private void Sort(FeedType feedType)
        {
            switch (feedType)
            {
                case FeedType.Auto:
                    var ordered = Auto.OrderBy(item => item.IsRead).ThenByDescending(item => item.PublishDate).ToList();
                    Auto.Clear();
                    foreach (var item in ordered)
                    {
                        Auto.Add(item);
                    }
                    break;
                case FeedType.People:
                    ordered = People.OrderBy(item => item.IsRead).ThenByDescending(item => item.PublishDate).ToList();
                    People.Clear();
                    foreach (var item in ordered)
                    {
                        People.Add(item);
                    }
                    break;
                case FeedType.Realt:
                    ordered = Realt.OrderBy(item => item.IsRead).ThenByDescending(item => item.PublishDate).ToList();
                    Realt.Clear();
                    foreach (var item in ordered)
                    {
                        Realt.Add(item);
                    }
                    break;
                case FeedType.Tech:
                    ordered = Tech.OrderBy(item => item.IsRead).ThenByDescending(item => item.PublishDate).ToList();
                    Tech.Clear();
                    foreach (var item in ordered)
                    {
                        Tech.Add(item);
                    }
                    break;
            }
        }

        public void SetFeedSelection(FeedItemViewModel feedItem)
        {
            if (Settings.IsMarkReadWhenOpen)
                feedItem.IsRead = true;
        }
    }
}