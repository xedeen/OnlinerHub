using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TestApp.ViewModels
{
    public enum FeedType
    {
        Tech,
        Auto,
        People,
        Realt
    }
    public class FeedItemViewModel : INotifyPropertyChanged
    {
        private FeedType _type;

        public FeedType FeedType
        {
            get
            {
                return _type;
            }
            set
            {
                if (value != _type)
                {
                    _type = value;
                    NotifyPropertyChanged("FeedType");
                }
            }
        }


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

        private string _uri;
        public string Uri
        {
            get
            {
                return _uri;
            }
            set
            {
                if (value != _uri)
                {
                    _uri = value;
                    NotifyPropertyChanged("Uri");
                }
            }
        }

        private string _imageUri;
        public string ImageUri
        {
            get
            {
                return _imageUri;
            }
            set
            {
                if (value != _imageUri)
                {
                    _imageUri = value;
                    NotifyPropertyChanged("ImageUri");
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