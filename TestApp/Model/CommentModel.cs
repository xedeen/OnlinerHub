using System;
using Onliner.OnlinerHub;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Onliner.Model
{
    public class CommentModel : INotifyPropertyChanged

    {
        private string _authorName;
        public string AuthorName
        {
            get { return _authorName; }
            set
            {
                if (_authorName != value)
                {
                    _authorName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Uri _avatarUrl;
        public Uri AvatarUrl
        {
            get { return _avatarUrl; }
            set
            {
                if (_avatarUrl != value)
                {
                    _avatarUrl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Uri _profile;
        public Uri Profile
        {
            get { return _profile; }
            set
            {
                if (_profile != value)
                {
                    _profile = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ContentDto _content;
        public ContentDto Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
