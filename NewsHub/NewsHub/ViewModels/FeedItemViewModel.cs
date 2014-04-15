using System;
using System.ComponentModel;

namespace NewsHub.ViewModels
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

        public string TitleU
        {
            get { return Title.ToUpper(); }
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

        private DateTime _publishDate;
        public DateTime PublishDate
        {
            get { return _publishDate; }
            set
            {
                if (value != _publishDate)
                {
                    _publishDate = value;
                    NotifyPropertyChanged("PublishDate");
                }
            }
        }

        public string PublishDateS
        {
            get
            {
                if (DateTime.Now.Subtract(PublishDate).TotalMinutes < 10)
                {
                    var mm = (int) DateTime.Now.Subtract(PublishDate).TotalMinutes;
                    return mm < 1
                        ? "Сегодня, только что"
                        : string.Format("Сегодня, {0} минут назад", mm);
                }
                if (PublishDate > DateTime.Today)
                    return string.Format("Сегодня, в {0}", PublishDate.ToString("HH:mm"));
                if (PublishDate > DateTime.Today.AddDays(-1))
                    return string.Format("Вчера, в {0}", PublishDate.ToString("HH:mm"));
                return PublishDate.ToString("dd.MM.yyyy, в HH:mm");
            }
        }

        private bool _isRead;
        public bool IsRead
        {
            get { return _isRead; }
            set
            {
                if (value != _isRead)
                {
                    _isRead = value;
                    NotifyPropertyChanged("IsRead");
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