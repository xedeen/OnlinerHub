using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

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
                //ArticleBrowser.Navigate(new Uri(uri));
                RetriveReadability(uri);
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        private void Readability_Completed(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            var serializer = new DataContractJsonSerializer(typeof(ReaderResult));
            ReaderResult current;

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                current = (ReaderResult) serializer.ReadObject(sr.BaseStream);
            }

            if (null != current)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    ArticleBrowser.NavigateToString(current.content);
                });
            }
        }


        private void RetriveReadability(string uri)
        {
            try
            {

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
            
        }
    }
}