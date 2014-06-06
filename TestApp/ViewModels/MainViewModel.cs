using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Onliner.Model;
using Onliner.Resources;

namespace Onliner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly object _readLock = new object();

        public MainViewModel()
        {
            IsLoading = false;
            this.Tech = new ObservableCollection<FeedItemViewModel>();
            this.Auto = new ObservableCollection<FeedItemViewModel>();
            this.People = new ObservableCollection<FeedItemViewModel>();
            this.Realt = new ObservableCollection<FeedItemViewModel>();
            Article = new ArticleViewModel();
        }
        
        


        public ObservableCollection<FeedItemViewModel> Tech { get; private set; }
        public ObservableCollection<FeedItemViewModel> Auto { get; private set; }
        public ObservableCollection<FeedItemViewModel> People { get; private set; }
        public ObservableCollection<FeedItemViewModel> Realt { get; private set; }

        private bool _storageFeedsRequested = false;

        public readonly AppSettings _appSettings = new AppSettings();

        public AppSettings Settings
        {
            get { return _appSettings; }
        }

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

        //private FeedType _currentSelectedPageType = FeedType.Auto;
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

        private FeedItemViewModel _selectedModel = null;
        public void SetFeedSelection(FeedItemViewModel item)
        {
            _selectedModel = item;
            if (Settings.IsMarkReadWhenOpen)
                _selectedModel.IsRead = true;
        }


        private Dictionary<string, ArticleViewModel> _localArticles = new Dictionary<string, ArticleViewModel>();
        private ArticleViewModel _article;
        public ArticleViewModel Article
        {
            get { return _article; }
            set
            {
                if (!value.Equals(_article))
                {
                    _article = value;
                    NotifyPropertyChanged("Article");
                }
            }
        }

        public void LoadArticle(string uri)
        {
            if (_localArticles.ContainsKey(Article.Uri))
            {
                Article.Content = _localArticles[Article.Uri].Content;
                Article.Title = _localArticles[Article.Uri].Title;
                return;
            }
            else
            {
                var cachedArticle = TryGetArticleFromStorage(Article);
                if (null != cachedArticle)
                {
                    Article.Content = cachedArticle.Content;
                    Article.Title = cachedArticle.Title;
                    return;
                }
            }

            IsLoading = true;
            try
            {
                var requestUrl =
                    string.Format(
                        "https://www.readability.com/api/content/v1/parser?url={0}&token=ecd1f4e3683b5bfbd41c02c20229011983d03671",
                        uri);
                var wc = new WebClient();
                wc.OpenReadCompleted += wc_GetArticleCompleted;
                wc.OpenReadAsync(new Uri(requestUrl));
            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(e.Message));
            }
        }

        private void wc_GetArticleCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    //Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(AppResources.ArticleRetrieveError));
                    Article.Title = _selectedModel.Title;
                    Article.Content = FixHtml(_selectedModel.Title, _selectedModel.Description);
                    return;
                }

                var serializer = new DataContractJsonSerializer(typeof (ReaderResult));
                ReaderResult current;
                using (var sr = new StreamReader(e.Result))
                {
                    current = (ReaderResult) serializer.ReadObject(sr.BaseStream);
                }

                Article.Title = current.title;
                Article.Content = FixHtml(current.title, current.content);
                if (!_localArticles.ContainsKey(Article.Uri))
                    _localArticles.Add(Article.Uri, Article.Clone());
            }
            finally
            {
                IsLoading = false;
            }
        }

        private string FixHtml(string title, string content)
        {
            return
                "<!doctype html><html><head><style type=\"text/css\">" +
                (DarkThemeApplied
                    ? ".article_title{width:100%;background-color:#444; padding-bottom:16px;} "
                    : ".article_title{width:100%;background-color:silver; padding-bottom:16px;} ") +
                    ""+
                "body {font-family: " + Settings.ArticleFont + "; "
                + "font-size: " + FromFontSize(Settings.ArticleFontSize) + "; "
                + (DarkThemeApplied
                    ? " background-color: black ;color:white; "
                    : string.Empty)
                + "}</style></head><title>"
                + title + "</title><body"
                + string.Format(" font-family: \"{0}\"> ", Settings.ArticleFont) +
                string.Format("<div class = \"article_title\"><h3>{0}</h3></div>", title)
                + content + "</body></html>";

        }

        private string FromFontSize(string articleFontSize)
        {
            switch (articleFontSize)
            {
                case "x-small":
                    return "9px";
                case "small":
                    return "13px";
                case "medium":
                    return "16px";
                case "large":
                    return "22px";
                case "x-large":
                    return "24px";
            }
            return "16px";
        }

        public bool DarkThemeApplied
        {
            get
            {
                return (Visibility) Application.Current.Resources
                    ["PhoneLightThemeVisibility"] != Visibility.Visible;
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

        private void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
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

        private void Sort(bool sortAll=false)
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


        public void Save()
        {
            SaveFeeds();
            SaveArticles();
        }

        private void SaveFeeds()
        {
            var saveReadFeeds = !Settings.IsDeleteReadArticles;
            lock (_readLock)
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists("Feeds"))
                        store.CreateDirectory("Feeds");

                    foreach (var model in Auto.Union(Tech).Union(Realt).Union(People))
                    {
                        if (saveReadFeeds || model.IsRead == false)
                        {
                            var fileName = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.Uri)) +
                                           ".feed";
                            fileName = Path.Combine("Feeds", fileName);
                            Save(store, fileName, model);
                        }
                    }
                }
            }
        }

        public void ClearCache()
        {
            try
            {
                lock (_readLock)
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (store.DirectoryExists("Feeds"))
                        {
                            foreach (var fileName in store.GetFileNames(Path.Combine("Feeds", "*.*")))
                            {
                                store.DeleteFile(Path.Combine("Feeds", fileName));
                            }
                            store.DeleteDirectory("Feeds");
                        }
                        if (store.DirectoryExists("Articles"))
                        {
                            foreach (var fileName in store.GetFileNames(Path.Combine("Articles", "*.*")))
                            {
                                store.DeleteFile(Path.Combine("Articles", fileName));
                            }
                            store.DeleteDirectory("Articles");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show("Error clearing cache. Try again later!"));
            }
        }

        private void SaveArticles()
        {
            lock (_readLock)
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists("Articles"))
                        store.CreateDirectory("Articles");

                    foreach (var model in _localArticles.Values)
                    {
                        var fileName = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.Uri)) +
                                       ".article";
                        fileName = Path.Combine("Articles", fileName);
                        Save(store, fileName, model);
                    }
                }
            }
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


        private ArticleViewModel TryGetArticleFromStorage(ArticleViewModel article)
        {
            lock (_readLock)
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists("Articles"))
                        store.CreateDirectory("Articles");

                    var fileName = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(article.Uri)) +
                                      ".article";

                    if (!store.FileExists(Path.Combine("Articles", fileName)))
                        return null;

                    var localArticle = Load<ArticleViewModel>(store, Path.Combine("Articles", fileName));
                    if (!_localArticles.ContainsKey(localArticle.Uri))
                        _localArticles.Add(localArticle.Uri, localArticle);
                    return localArticle;
                }
            }
        }

        public static void Save<T>(IsolatedStorageFile store, string fileName, T item)
        {
            using (var fileStream = new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write,
                FileShare.None, store))
            {
                var serializer = new DataContractSerializer(typeof (T));
                serializer.WriteObject(fileStream, item);
            }
        }

        public T Load<T>(IsolatedStorageFile store, string fileName)
        {
            using (var fileStream = new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read,
                FileShare.None, store))
            {
                var serializer = new DataContractSerializer(typeof (T));
                return (T) serializer.ReadObject(fileStream);
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

        private string SelectItemFromFeed(ObservableCollection<FeedItemViewModel> feedCollection, bool isNextItem)
        {
            var currIndex = 0;
            currIndex += feedCollection.TakeWhile(item => item.Uri != _selectedModel.Uri).Count();
            if ((!isNextItem && currIndex > 0) || (isNextItem && currIndex < feedCollection.Count - 1))
            {
                var newIndex = isNextItem ? currIndex + 1 : currIndex - 1;
                SetFeedSelection(feedCollection[newIndex]);
                return feedCollection[newIndex].Uri;
            }
            return null;
        }

        public string GetPrevUri()
        {
            if (null == _selectedModel)
                return null;

            switch (_selectedModel.FeedType)
            {
                case FeedType.Auto:
                    return SelectItemFromFeed(Auto, false);
                case FeedType.People:
                    return SelectItemFromFeed(People, false);
                case FeedType.Realt:
                    return SelectItemFromFeed(Realt, false);
                case FeedType.Tech:
                    return SelectItemFromFeed(Tech, false);
            }
            return null;
        }

        public string GetNextUri()
        {
            if (null == _selectedModel)
                return null;

            switch (_selectedModel.FeedType)
            {
                case FeedType.Auto:
                    return SelectItemFromFeed(Auto, true);
                case FeedType.People:
                    return SelectItemFromFeed(People, true);
                case FeedType.Realt:
                    return SelectItemFromFeed(Realt, true);
                case FeedType.Tech:
                    return SelectItemFromFeed(Tech, true);
            }
            return null;
        }
    }
}