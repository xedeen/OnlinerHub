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
using NewsHub.Annotations;
using NewsHub.OnlinerHub;

namespace NewsHub.ViewModels
{
    public class ArticleViewModel : LoadViewModelBase
    {
        private readonly object _readLock = new object();
        private readonly Dictionary<string, ArticleItemViewModel> _memCachedArticles = new Dictionary<string, ArticleItemViewModel>();

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
            LoadHeader();
            LoadPage(0);
        }

        private void LoadHeader()
        {
            var client = new OnlinerHubClient();
            client.GetHeaderCompleted += GetHeaderCompleted;
            client.GetHeaderAsync(Article.Uri);
        }

        public void LoadPage(int pageNumber)
        {
            IsLoading = true;
            var client = new OnlinerHubClient();
            client.GetContentCompleted += GetContentPageCompleted;
            client.GetContentAsync(Article.Uri, pageNumber);
        }

        private void GetContentPageCompleted(object sender, GetContentCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (!e.Result.previous_page_cursor.HasValue)
                            this.Article.ContentCollection.Clear();
                        if (null != e.Result.paragraphs)
                        {
                            foreach (var p in e.Result.paragraphs)
                            {
                                this.Article.ContentCollection.Add(p);
                            }
                        }
                        Article.EOF = null == e.Result.next_page_cursor;
                        if (Article.EOF && _memCachedArticles.ContainsKey(Article.Uri))
                            _memCachedArticles.Add(Article.Uri, Article.Clone());
                    });
                }
            }
            catch (Exception exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(
                    () => MessageBox.Show("Network error occured " + exception.Message));
            }
            finally
            {
                IsLoading = false;
            }

        }

        private void GetHeaderCompleted(object sender, GetHeaderCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Article.Title = e.Result.Title;
                        if (null != e.Result.Tags)
                        {
                            foreach (var tag in e.Result.Tags)
                            {
                                this.Article.TagsCollection.Add(tag);
                            }
                        }
                    });
                }
            }
            catch (Exception exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(
                    () => MessageBox.Show("Network error occured " + exception.Message));
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
