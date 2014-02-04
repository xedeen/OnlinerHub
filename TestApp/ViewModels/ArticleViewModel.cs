using System;
using System.ComponentModel;

namespace Onliner.ViewModels
{
    public class ArticleViewModel : INotifyPropertyChanged
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

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
               
               _content = value;
               NotifyPropertyChanged("Content");
               
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