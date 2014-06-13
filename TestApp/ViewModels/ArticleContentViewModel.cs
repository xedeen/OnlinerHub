using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows;
using Onliner.Model;
using Onliner.OnlinerHub;

namespace Onliner.ViewModels
{
    public class ArticleContentViewModel : INotifyPropertyChanged
    {

        #region Properties
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

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private Uri _uri;
        public Uri Uri
        {
            get { return _uri; }
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    NotifyPropertyChanged("Uri");
                }
            }
        }

        public ObservableCollection<ArticleParagraphModel> ContentCollection
        {
            get;
            private set;
        }

        public readonly AppSettings _appSettings = new AppSettings();

        public AppSettings Settings
        {
            get { return _appSettings; }
        }

        #endregion

        public ArticleContentViewModel()
        {
            IsLoading = false;
            this.ContentCollection = new ObservableCollection<ArticleParagraphModel>();
        }

        public void LoadContentPage(int pageNumber)
        {
            if (pageNumber == 0) this.ContentCollection.Clear();
            IsLoading = true;

            var client = new OnlinerHubClient();
            client.GetContentXamlCompleted += ContentPageCallback ;
            client.GetContentXamlAsync(this.Uri.ToString());
        }

        private void ContentPageCallback(object sender, GetContentXamlCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        //if (!e.Result.previous_page_cursor.HasValue)
                            this.ContentCollection.Clear();
                        if (null != e.Result)
                        {
                            foreach (var p in e.Result)
                            {
                                this.ContentCollection.Add(new ArticleParagraphModel
                                {
                                    ContentType = p.ContentType,
                                    Xaml = p.Xaml,
                                    Link = p.LatestLink
                                });
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
