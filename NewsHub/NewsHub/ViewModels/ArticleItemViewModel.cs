using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NewsHub.Annotations;
using NewsHub.OnlinerHub;

namespace NewsHub.ViewModels
{
    public class ArticleItemViewModel : INotifyPropertyChanged
    {
        public ArticleItemViewModel Clone()
        {
            return (ArticleItemViewModel)this.MemberwiseClone();
        }

        public ArticleItemViewModel()
        {
            ContentCollection = new ObservableCollection<ParagraphBase>();
            TagsCollection = new ObservableCollection<Tag>();
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public ObservableCollection<ParagraphBase> ContentCollection
        {
            get;
            private set;
        }

        public ObservableCollection<Tag>  TagsCollection
        {
            get;
            private set;
        }

        private string _uri;
        public string Uri
        {
            get { return _uri; }
            set
            {
                if (value != _uri)
                {
                    _uri = value;
                    NotifyPropertyChanged("Uri");
                }
            }
        }

        private bool _EOF;

        public bool EOF
        {
            get { return _EOF; }
            set { _EOF = value;
                NotifyPropertyChanged("EOF");
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
