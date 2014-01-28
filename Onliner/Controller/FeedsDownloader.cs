using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Xml;
using Onliner.ViewModels;

namespace Onliner.Controller
{
    public delegate void EndDownloadCallback(IEnumerable<FeedItemViewModel> items, string feedType);

    public class FeedsDownloadedEventArgs : EventArgs
    {
        public IEnumerable<FeedItemViewModel> Items { get; set; }
        public string feedType { get; set; }
    }

    internal class FeedDownloadInfo
    {
        public string FeedType = string.Empty;
        public EndDownloadCallback endDownloadCallback;

        public FeedDownloadInfo(string feedType, EndDownloadCallback callback)
        {
            FeedType = feedType;
            endDownloadCallback = callback;
        }
    }

    public class FeedsDownloader
    {
        /// <summary>
        /// Event to fire to trigger the download complete notificatio to clients.
        /// </summary>
        internal event EventHandler<FeedsDownloadedEventArgs> FeedsDownloaded;

        /// <summary>
        /// Constructor
        /// </summary>
        public FeedsDownloader()
        {
        }

        private AutoResetEvent _downloadFinished = new AutoResetEvent(false);
        private List<FeedItemViewModel> _currentFeeds;

        /// <summary>
        /// Async download start, This will start a new thread and schedule page download task.
        /// </summary>
        public void StartDownloadFeedsAsync(string feedType)
        {
            var pageInfo = new FeedDownloadInfo(feedType, new EndDownloadCallback(EndDownloadFeed));
            ThreadPool.QueueUserWorkItem(new WaitCallback(FeedDownload), pageInfo);
        }

        /// <summary>
        /// Method that will be called by the thread pool thread when download completes.
        /// </summary>
        public void EndDownloadFeed(IEnumerable<FeedItemViewModel> items, string feedType)
        {
            var downloadedFeeds = new FeedsDownloadedEventArgs {Items = items, feedType = feedType};
            FeedsDownloaded(this, downloadedFeeds);
        }

        /// <summary>
        /// Method which downloads feeds given its type and return the downloaded feeds using callback provided.
        /// </summary>
        /// <param name="state"></param>
        private void FeedDownload(object state)
        {
            var feedType = ((FeedDownloadInfo) state).FeedType;
            var endDownloadCallback = ((FeedDownloadInfo) state).endDownloadCallback;

            if (endDownloadCallback == null)
                throw new ArgumentNullException("endDownloadCallback");

            var items = GetFeedsInner(feedType);
            endDownloadCallback(items, feedType);
        }




        private void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
            }
            try
            {
                using (var reader = XmlReader.Create(e.Result))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    _currentFeeds = new List<FeedItemViewModel>();

                    foreach (var item in feed.Items)
                    {
                        _currentFeeds.Add(new FeedItemViewModel
                        {
                            Description = item.Summary.Text,
                            Link = item.Links[0].Uri,
                            PubDate = item.PublishDate,
                            Title = item.Title.Text
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(exception.Message));
            }
            finally
            {
                _downloadFinished.Set();
            }
        }

        private IEnumerable<FeedItemViewModel> GetFeedsInner(string feedType)
        {
            var wc = new WebClient();
            wc.OpenReadCompleted += wc_OpenReadCompleted;
            wc.OpenReadAsync(new Uri(string.Format("http://{0}.onliner.by/feed", feedType)));
            if (_downloadFinished.WaitOne(10000))
                return _currentFeeds;
            
            _currentFeeds = null;
            return Enumerable.Empty<FeedItemViewModel>();
        }
    }


    public class RssTextTrimmer : IValueConverter
    {
        // Clean up text fields from each SyndicationItem. 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            int maxLength = 200;
            int strLength = 0;
            string fixedString = "";

            // Remove HTML tags and newline characters from the text, and decodes HTML encoded characters. 
            // This is a basic method. Additional code would be needed to more thoroughly  
            // remove certain elements, such as embedded Javascript. 

            // Remove HTML tags. 
            fixedString = Regex.Replace(value.ToString(), "<[^>]+>", string.Empty);

            // Remove newline characters
            fixedString = fixedString.Replace("\r", "").Replace("\n", "");

            // Remove encoded HTML characters
            fixedString = HttpUtility.HtmlDecode(fixedString);

            strLength = fixedString.ToString().Length;

            // Some feed management tools include an image tag in the Description field of an RSS feed, 
            // so even if the Description field (and thus, the Summary property) is not populated, it could still contain HTML. 
            // Due to this, after we strip tags from the string, we should return null if there is nothing left in the resulting string. 
            if (strLength == 0)
            {
                return null;
            }

            // Truncate the text if it is too long. 
            else if (strLength >= maxLength)
            {
                fixedString = fixedString.Substring(0, maxLength);

                // Unless we take the next step, the string truncation could occur in the middle of a word.
                // Using LastIndexOf we can find the last space character in the string and truncate there. 
                fixedString = fixedString.Substring(0, fixedString.LastIndexOf(" "));
            }

            fixedString += "...";

            return fixedString;
        }

        // This code sample does not use TwoWay binding and thus, we do not need to flesh out ConvertBack.  
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
