using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.ViewModels
{
    public class AppSettings : INotifyPropertyChanged
    {
        private IsolatedStorageSettings settings;
        private const string isOnlinerTechSubscribed = "isOnlinerTechSubscribed";
        private const string isOnlinerAutoSubscribed = "isOnlinerAutoSubscribed";
        private const string isOnlinerPeopleSubscribed = "isOnlinerPeopleSubscribed";
        private const string isOnlinerRealtSubscribed = "isOnlinerRealtSubscribed";
        private const string isMinimalFeedStyle = "isMinimalFeedStyle";
        private const string articleFontName = "articleFontName";
        private const string articleFontSize = "articleFontSize";
        private const string isMarkReadWhenOpen = "isMarkReadWhenOpen";
        private const string isDeleteReadArticles = "isDeleteReadArticles";
        
        private const bool subscribtionSettingDefault = true;

        
        public ObservableCollection<string> FontNames { get; set; }
        public ObservableCollection<string> FontSizes { get; set; }

        public AppSettings()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
            FontNames = new ObservableCollection<string> {"segoe wp", "calibri", "cambria", "georgia", "verdana"};
            FontSizes = new ObservableCollection<string> { "x-small", "small", "medium", "large", "x-large" };
        }

        

        public string ArticleFont
        {
            get { return GetValueOrDefault<string>(articleFontName, "segoe wp"); }
            set
            {
                if (AddOrUpdateValue(articleFontName, value))
                {
                    Save();
                    NotifyPropertyChanged("ArticleFont");
                }
            }
        }

        public string ArticleFontSize
        {
            get { return GetValueOrDefault<string>(articleFontSize, "medium"); }
            set
            {
                if (AddOrUpdateValue(articleFontSize, value))
                {
                    Save();
                    NotifyPropertyChanged("ArticleFontSize");
                }
            }
        }

        public bool IsMarkReadWhenOpen
        {
            get { return GetValueOrDefault<bool>(isMarkReadWhenOpen, subscribtionSettingDefault); }

            set
            {
                if (AddOrUpdateValue(isMarkReadWhenOpen, value))
                {
                    Save();
                    NotifyPropertyChanged("IsMarkReadWhenOpen");
                }
            }
        }

        public bool IsDeleteReadArticles
        {
            get { return GetValueOrDefault<bool>(isDeleteReadArticles, !subscribtionSettingDefault); }

            set
            {
                if (AddOrUpdateValue(isDeleteReadArticles, value))
                {
                    Save();
                    NotifyPropertyChanged("IsDeleteReadArticles");
                }
            }
        }

        public bool IsOnlinerTechSubscribed
        {
            get { return GetValueOrDefault<bool>(isOnlinerTechSubscribed, subscribtionSettingDefault); }

            set
            {
                if (AddOrUpdateValue(isOnlinerTechSubscribed, value))
                {
                    Save();
                    NotifyPropertyChanged("IsOnlinerTechSubscribed");
                }
            }
        }

        public bool IsOnlinerAutoSubscribed
        {
            get { return GetValueOrDefault<bool>(isOnlinerAutoSubscribed, subscribtionSettingDefault); }

            set
            {
                if (AddOrUpdateValue(isOnlinerAutoSubscribed, value))
                {
                    Save();
                    NotifyPropertyChanged("IsOnlinerAutoSubscribed");
                }
            }
        }

        public bool IsOnlinerPeopleSubscribed
        {
            get { return GetValueOrDefault<bool>(isOnlinerPeopleSubscribed, subscribtionSettingDefault); }

            set
            {
                if (AddOrUpdateValue(isOnlinerPeopleSubscribed, value))
                {
                    Save();
                    NotifyPropertyChanged("IsOnlinerPeopleSubscribed");
                }
            }
        }

        public bool IsOnlinerRealtSubscribed
        {
            get { return GetValueOrDefault<bool>(isOnlinerRealtSubscribed, subscribtionSettingDefault); }

            set
            {
                if (AddOrUpdateValue(isOnlinerRealtSubscribed, value))
                {
                    Save();
                    NotifyPropertyChanged("IsOnlinerRealtSubscribed");
                }
            }
        }

        public bool IsMinimalFeedStyle
        {
            get { return GetValueOrDefault<bool>(isMinimalFeedStyle, !subscribtionSettingDefault); }

            set
            {
                if (AddOrUpdateValue(isMinimalFeedStyle, value))
                {
                    Save();
                    NotifyPropertyChanged("IsMinimalFeedStyle");
                }
            }
        }


        #region generics
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        public void Save()
        {
            settings.Save();
        }
        #endregion

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
