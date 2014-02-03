using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using TestApp.Resources;

namespace TestApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
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

        private FeedType _currentType = FeedType.Auto;

        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");

            }
        }


        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData(FeedType feedType)
        {
            IsLoading = true;
            var wc = new WebClient();
            wc.OpenReadCompleted += wc_OpenReadCompleted;
            _currentType = feedType;

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

        private void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
            }
            try
            {
                switch (_currentType)
                {
                    case FeedType.Auto:
                        Auto.Clear();
                        break;
                    case FeedType.People:
                        People.Clear();
                        break;
                    case FeedType.Realt:
                        Realt.Clear();
                        break;
                    case FeedType.Tech:
                        Tech.Clear();
                        break;
                }

                using (var reader = XmlReader.Create(e.Result))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    var items = new List<FeedItemViewModel>();
                    foreach (SyndicationItem item in feed.Items)
                    {
                        SyndicationElementExtension media =
                            item.ElementExtensions.FirstOrDefault(ee => ee.OuterName == "thumbnail");
                        XAttribute attribute = media == null ? null : media.GetObject<XElement>().Attribute("url");

                        switch (_currentType)
                        {
                            case FeedType.Auto:
                                Auto.Add(new FeedItemViewModel
                                {
                                    Description = item.Summary.Text,
                                    Uri = item.Links[0].Uri.ToString(),
                                    Title = item.Title.Text,
                                    ImageUri = (null != attribute) ? attribute.Value : string.Empty,
                                    FeedType = _currentType
                                });
                                break;
                            case FeedType.People:
                                People.Add(new FeedItemViewModel
                                {
                                    Description = item.Summary.Text,
                                    Uri = item.Links[0].Uri.ToString(),
                                    Title = item.Title.Text,
                                    ImageUri = (null != attribute) ? attribute.Value : string.Empty,
                                    FeedType = _currentType
                                });
                                break;
                            case FeedType.Realt:
                                Realt.Add(new FeedItemViewModel
                                {
                                    Description = item.Summary.Text,
                                    Uri = item.Links[0].Uri.ToString(),
                                    Title = item.Title.Text,
                                    ImageUri = (null != attribute) ? attribute.Value : string.Empty,
                                    FeedType = _currentType
                                });
                                break;
                            case FeedType.Tech:
                                Tech.Add(new FeedItemViewModel
                                {
                                    Description = item.Summary.Text,
                                    Uri = item.Links[0].Uri.ToString(),
                                    Title = item.Title.Text,
                                    ImageUri = (null != attribute) ? attribute.Value : string.Empty,
                                    FeedType = _currentType
                                });
                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(exception.Message));
            }
            finally
            {
                IsLoading = false;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}