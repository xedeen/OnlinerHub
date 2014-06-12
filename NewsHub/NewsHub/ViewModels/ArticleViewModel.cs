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
using HtmlAgilityPack;
using NewsHub.Commands;
using NewsParser.Controller;
using NewsParser.Model;
using NewsParser.Model.Events;


namespace NewsHub.ViewModels
{
    public class ArticleViewModel : LoadViewModelBase
    {
        private readonly object _readLock = new object();
        private readonly Dictionary<string, ArticleItemViewModel> _memCachedArticles = new Dictionary<string, ArticleItemViewModel>();
        public ICommand ItemClickCommand { get; private set; }
        private readonly OnlinerLoader _loader = new OnlinerLoader("xedin", "spectrum");
        private HtmlAgilityPack.HtmlDocument _pageDocument;

        public ArticleViewModel()
        {
            ItemClickCommand = new RelayCommand(Test);
            _loader.LoadComplete += OnLoadComplete;
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

        private Visibility _commentsVisible;
        public Visibility CommentsVisible
        {
            get { return _commentsVisible; }
            private set
            {
                _commentsVisible = value;
                NotifyPropertyChanged("CommentsVisible");
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
            LoadPage(0);
        }

        public void LoadPage(int pageNumber)
        {
            IsLoading = true;
            LoadPage();
            this.CommentsVisible = Visibility.Collapsed;
        }

        public void LoadPage()
        {
            _loader.Load(Article.Uri);
        }

        private void OnLoadComplete(object sender, LoadCompleteEventArgs args)
        {
            if (args.Success)
            {
                _pageDocument = new HtmlDocument();
                _pageDocument.LoadHtml(args.Page.DocumentNode.InnerHtml);
                var parser = new OnlinerParser(args.Page);
                parser.ArticleParsed += OnArticleProcessed;
                parser.ParseArticleAsync();
            }
        }

        private void OnCommentsParsed(object sender, CommentsCompleteEventArgs args)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.Article.CommentsCollection.Clear();
                    if (null != args.Comments)
                    {
                        foreach (var c in args.Comments)
                        {
                            this.Article.CommentsCollection.Add(c);
                        }
                        this.CommentsVisible = Visibility.Visible;
                    }
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

        private void OnArticleProcessed(object sender, ArticleCompleteEventArgs args)
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
                        this.Article.ContentCollection.Add(
                            new ParagraphViewModel
                            {
                                Content = new Footer(),
                                CommentsCmd = new RelayCommand(CommentCmdClicked)
                            });
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

        private void CommentCmdClicked(object obj)
        {
            if (null != _pageDocument)
            {
                IsLoading = true;
                var parser = new OnlinerParser(_pageDocument);
                parser.CommentsParsed += OnCommentsParsed;
                parser.ParseCommentsAsync();
            }
        }
    }
}
