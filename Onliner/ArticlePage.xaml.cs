using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Onliner.Annotations;
using Onliner.OnlinerHub;

namespace Onliner
{
    public class ReaderResult
    {
        public string content { get; set; }
        public string domain { get; set; }
        public string author { get; set; }
        public string url { get; set; }
        public string short_url { get; set; }
        public string title { get; set; }
        public string excerpt { get; set; }
        public string direction { get; set; }
        public int? word_count { get; set; }
        public string total_pages { get; set; }
        public string date_published { get; set; }
        public string dek { get; set; }
        public string lead_image_url { get; set; }
        public int? next_page_id { get; set; }
        public int? rendered_pages { get; set; }
    }


    public partial class ArticlePage : PhoneApplicationPage
    {
        private class WrapperContext : INotifyPropertyChanged
        {
            public List<CommentDto> Comments { get; set; }
            private string _title;

            public string Title
            {
                get { return _title; }
                set { _title = value;NotifyPropertyChanged("Title"); }
            }

            public string Uri { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }

            protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
            {
                if (object.Equals(storage, value)) return false;

                storage = value;
                this.NotifyPropertyChanged(propertyName);

                return true;
            }
        }

        private WrapperContext _context = new WrapperContext();

        public ArticlePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string uri = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("uri", out uri))
            {
                _context.Uri = uri;
                RetriveReadability(uri);
                DataContext = _context;
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        private void Readability_Completed(IAsyncResult result)
        {
            var request = (HttpWebRequest) result.AsyncState;
            var response = (HttpWebResponse) request.EndGetResponse(result);
            var serializer = new DataContractJsonSerializer(typeof (ReaderResult));
            ReaderResult current;

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                current = (ReaderResult) serializer.ReadObject(sr.BaseStream);
            }
            var text =
                //"<html><head><style type=\"text/css\">.b-posts-1-item__text{background-color:black;color:white;}.b-posts-1-item__image{background-color:black;color:white;}div{background-color:black;color:white;}</style></head><body background-color=\"black>\"" +
                "<html><head><style type=\"text/css\">body {background-color: black ;color:white} </style></head><body>"+
                current.content + "</body></html>";


            this.Dispatcher.BeginInvoke(() =>
            {
                _context.Title = current.title;
                DataContext = null;
                DataContext = _context;
                ArticleBrowser.NavigateToString(text);
            });

        }


        private void RetriveReadability(string uri)
        {
            try
            {
                ArticleBrowser.Opacity = 0;
                var requestUrl =
                    string.Format(
                        "https://www.readability.com/api/content/v1/parser?url={0}&token=ecd1f4e3683b5bfbd41c02c20229011983d03671",
                        uri);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.BeginGetResponse(Readability_Completed, httpWebRequest);
            }
            catch (Exception e)
            {
            }

        }


        private void OnPageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (pivot1.SelectedItem as PivotItem);
            if (null != item && item.Name == "CommentItem")
            {
                var client = new OnlinerHubClient();
                client.GetCommentsCompleted += OnCommentsReceived;
                client.GetCommentsAsync(_context.Uri, 0);
            }
        }

        void OnCommentsReceived(object sender, GetCommentsCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                ProcessCommentsPage(e.Result);
            });
        }

        private void ProcessCommentsPage(CommentsPageDto commentsPage)
        {
            DataContext = null;
            if (commentsPage.previous_page_cursor == null)
                _context.Comments = new List<CommentDto>();

            foreach (var item in commentsPage.comments)
            {
                _context.Comments.Add(item);
            }
            DataContext = _context;
        }

        private void ArticleBrowser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            ArticleBrowser.Opacity = 1;
        }
    }
}