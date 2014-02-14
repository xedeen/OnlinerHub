using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.ViewModels
{
    public class AppSettings
    {
        private IsolatedStorageSettings settings;
        private const string isOnlinerTechSubscribed = "isOnlinerTechSubscribed";
        private const string isOnlinerAutoSubscribed = "isOnlinerAutoSubscribed";
        private const string isOnlinerPeopleSubscribed = "isOnlinerPeopleSubscribed";
        private const string isOnlinerRealtSubscribed = "isOnlinerRealtSubscribed";

        private const bool subscribtionSettingDefault = true;


        public AppSettings()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public bool IsOnlinerTechSubscribed
        {
            get { return GetValueOrDefault<bool>(isOnlinerTechSubscribed, subscribtionSettingDefault); }

            set
            {
                if (AddOrUpdateValue(isOnlinerTechSubscribed, value))
                {
                    Save();
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


    }
}
