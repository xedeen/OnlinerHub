using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Onliner.Controller;
using Onliner.Resources;

namespace Onliner.ViewModels
{
    public class FeedViewModel : INotifyPropertyChanged
    {
        private readonly FeedsDownloader _feedDownloader = new FeedsDownloader();

        public FeedViewModel()
        {
            _feedDownloader.FeedsDownloaded += OnFinishDownloadFeeds;
        }

        public event EventHandler<FeedsDownloadedEventArgs> FeedsDownloaded;

        private ObservableCollection<FeedItemViewModel> _itemsAuto = new ObservableCollection<FeedItemViewModel>();
        private ObservableCollection<FeedItemViewModel> _itemsTech = new ObservableCollection<FeedItemViewModel>();
        private ObservableCollection<FeedItemViewModel> _itemsPeople = new ObservableCollection<FeedItemViewModel>();
        private ObservableCollection<FeedItemViewModel> _itemsRealt = new ObservableCollection<FeedItemViewModel>();

        public ObservableCollection<FeedItemViewModel> ItemsAuto
        {
            get
            {
                if (_itemsAuto == null)
                {
                    //LoadData(); //Don't await...
                    //Items=new ObservableCollection<FeedItemViewModel>();
                }
                return _itemsAuto;
            }
            private set { SetProperty(ref _itemsAuto, value); }
        }

        public ObservableCollection<FeedItemViewModel> ItemsTech
        {
            get
            {
                return _itemsTech;
            }
            private set { SetProperty(ref _itemsTech, value); }
        }

        public ObservableCollection<FeedItemViewModel> ItemsPeople
        {
            get
            {
                return _itemsPeople;
            }
            private set { SetProperty(ref _itemsPeople, value); }
        }

        public ObservableCollection<FeedItemViewModel> ItemsRealt
        {
            get
            {
                return _itemsRealt;
            }
            private set { SetProperty(ref _itemsRealt, value); }
        }

        public void SetItems(IEnumerable<FeedItemViewModel> newItems, string feedType)
        {
            IsLoading = false;
            if (feedType.ToLower().Equals("auto")) ItemsAuto = new ObservableCollection<FeedItemViewModel>(newItems);
            if (feedType.ToLower().Equals("tech")) ItemsTech = new ObservableCollection<FeedItemViewModel>(newItems);
            if (feedType.ToLower().Equals("people")) ItemsPeople = new ObservableCollection<FeedItemViewModel>(newItems);
            if (feedType.ToLower().Equals("realt")) ItemsRealt = new ObservableCollection<FeedItemViewModel>(newItems);
        }


        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                SetProperty(ref _isLoading, value);
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

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.NotifyPropertyChanged(propertyName);

            return true;
        }

        public void StartRetrieveItems(string feedType)
        {
            _feedDownloader.StartDownloadFeedsAsync(feedType);
        }

        void OnFinishDownloadFeeds(object sender, FeedsDownloadedEventArgs e)
        {
            if (null != FeedsDownloaded)
            {
                FeedsDownloaded.Invoke(this, e);
            }
        }

    }
}