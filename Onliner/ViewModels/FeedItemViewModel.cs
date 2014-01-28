using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Onliner.ViewModels
{
    public class FeedItemViewModel : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        private Uri _link;
        public Uri Link
        {
            get
            {
                return _link;
            }
            set
            {
                if (value != _link)
                {
                    _link = value;
                    NotifyPropertyChanged("Link");
                }
            }
        }

        private DateTimeOffset _pubDate;
        public DateTimeOffset PubDate
        {
            get
            {
                return _pubDate;
            }
            set
            {
                if (value != _pubDate)
                {
                    _pubDate = value;
                    NotifyPropertyChanged("PubDate");
                }
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