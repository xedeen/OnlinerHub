using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NewsHub.Commands;
using NewsParser.Controller;
using NewsParser.Model.Events;


namespace NewsHub.ViewModels
{
    public class ArticleViewModel : LoadViewModelBase
    {
        private readonly object _readLock = new object();
        private readonly Dictionary<string, ArticleItemViewModel> _memCachedArticles = new Dictionary<string, ArticleItemViewModel>();
        public ICommand ItemClickCommand { get; private set; }

        public ArticleViewModel()
        {
            ItemClickCommand = new RelayCommand(Test);
        }

        private void Test(object obj)
        {
            var b = obj;
        }

        private ArticleItemViewModel _article;
        public ArticleItemViewModel Article
        {
            get { return _article; }
            private set
            {
                _article = value;
                NotifyPropertyChanged("Article");
            }
        }


        public void LoadData(string uri)
        {
            if (_memCachedArticles.ContainsKey(uri))
            {
                Article = _memCachedArticles[uri];
                return;
            }
            else
            {
                var cachedArticle = TryGetArticleFromStorage(uri);
                if (null != cachedArticle)
                {
                    Article = cachedArticle;
                    return;
                }
            }
            Article = new ArticleItemViewModel {Uri = uri};
            //LoadHeader();
            LoadPage(0);
        }

        private void LoadHeader()
        {
            //var client = new OnlinerHubClient();
            //client.GetHeaderCompleted += GetHeaderCompleted;
            //client.GetHeaderAsync(Article.Uri);
        }

        public void LoadPage(int pageNumber)
        {
            IsLoading = true;
            LoadPage();
            //var client = new OnlinerHubClient();
            //client.GetContentCompleted += GetContentPageCompleted;
            //client.GetContentAsync(Article.Uri, pageNumber);
        }

        public void LoadPage()
        {
            var p = new OnlinerParser();
            p.ParseComplete += OnArticleProcessed;
            p.Parse(Article.Uri);
        }

        private void OnArticleProcessed(object sender, ParseCompleteEventArgs args)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.Article.ContentCollection.Clear();
                    if (null != args.Article.Content)
                    {
                        foreach (var p in args.Article.Content)
                        {
                            this.Article.ContentCollection.Add(
                                new ParagraphViewModel {Content = p}
                                );
                        }
                    }
                    if (null != args.Article.Header.Tags)
                    {
                        foreach (var tag in args.Article.Header.Tags)
                        {
                            this.Article.TagsCollection.Add(tag);
                        }
                    }
                    Article.Title = args.Article.Header.Title;
                    Article.HeaderImgUri = args.Article.Header.Image.SourceUrl;
                    Article.EOF = true;
                    if (Article.EOF && !_memCachedArticles.ContainsKey(Article.Uri))
                        _memCachedArticles.Add(Article.Uri, Article.Clone());
                });
            }
            catch (Exception exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(
                    () => MessageBox.Show("Network error occured " + exception.Message));
            }
            finally
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { IsLoading = false; });
            }
        }

        private ArticleItemViewModel TryGetArticleFromStorage(string uri)
        {
            lock (_readLock)
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists("Articles"))
                        store.CreateDirectory("Articles");

                    var fileName = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(uri)) +
                                   ".article";

                    if (!store.FileExists(Path.Combine("Articles", fileName)))
                        return null;

                    var localArticle = Load<ArticleItemViewModel>(store, Path.Combine("Articles", fileName));
                    if (!_memCachedArticles.ContainsKey(localArticle.Uri))
                        _memCachedArticles.Add(localArticle.Uri, localArticle);
                    return localArticle;
                }
            }
        }
    }
}
