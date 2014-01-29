using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Onliner.Contracts;
using Onliner.Controller;
using Onliner.Model;
using Onliner.OnlinerHub;

namespace Onliner.ViewModels
{
    public class ArticleViewModel : INotifyPropertyChanged
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

        private bool _eof = false;
        public bool EOF
        {
            get
            {
                return _eof;
            }
            set
            {
                _eof = value;
                NotifyPropertyChanged("EOF");

            }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            private set
            {
                if (_content != value)
                {
                    _content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            private set
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

        public ObservableCollection<CommentModel> CommentCollection
        {
            get;
            private set;
        }


        #endregion

        public ArticleViewModel()
        {
            IsLoading = false;
            this.CommentCollection=new ObservableCollection<CommentModel>();
        }

        public void LoadArticle(string uri)
        {
            Uri = new Uri(uri);
            IsLoading = true;
            try
            {
                var requestUrl =
                    string.Format(
                        "https://www.readability.com/api/content/v1/parser?url={0}&token=ecd1f4e3683b5bfbd41c02c20229011983d03671",
                        uri);

                var httpWebRequest = (HttpWebRequest) WebRequest.Create(requestUrl);
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.BeginGetResponse(ReadabilityCallback, httpWebRequest);
            }
            catch (Exception exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(
                    () => MessageBox.Show("Error retrieving article! " + exception.Message));
            }
        }

        public void LoadCommentPage(int pageNumber)
        {
            if (pageNumber == 1) this.CommentCollection.Clear();
            IsLoading = true;

            var client = new OnlinerHubClient();
            client.GetCommentsCompleted += CommentPageCallback ;
            client.GetCommentsAsync(this.Uri.ToString(), pageNumber);
        }

        private void CommentPageCallback(object sender, GetCommentsCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        foreach (var comment in e.Result.comments)
                        {
                            this.CommentCollection.Add(new CommentModel
                            {
                                AuthorName = comment.author.name,
                                AvatarUrl = new Uri(comment.author.avatar_source_uri),
                                Content = comment.content,
                                Profile = new Uri(comment.author.profile_uri)
                            });
                        }
                        EOF = null == e.Result.next_page_cursor;
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


        private void ReadabilityCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                var request = (HttpWebRequest) asynchronousResult.AsyncState;
                var response = (HttpWebResponse) request.EndGetResponse(asynchronousResult);
                var serializer = new DataContractJsonSerializer(typeof (ReadabilityResult));

                string text;
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var current = (ReadabilityResult) serializer.ReadObject(sr.BaseStream);
                    text =
                        "<html><head><style type=\"text/css\">body {background-color: black ;color:white} </style></head><body>" +
                        current.content + "</body></html>";
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.Content = text;
                });

            }
            catch (Exception exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(
                    () => MessageBox.Show("Error retrieving article! " + exception.Message));
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
