using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Onliner.Annotations;

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

        private OnlinerHub.ContentDto _content;
        public OnlinerHub.ContentDto Content
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
