using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NewsHub.ViewModels
{
    public abstract class LoadViewModelBase : INotifyPropertyChanged
    {
        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");

            }
        }

        public static void Save<T>(IsolatedStorageFile store, string fileName, T item)
        {
            using (var fileStream = new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write,
                FileShare.None, store))
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fileStream, item);
            }
        }

        public T Load<T>(IsolatedStorageFile store, string fileName)
        {
            using (var fileStream = new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read,
                FileShare.None, store))
            {
                var serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(fileStream);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
