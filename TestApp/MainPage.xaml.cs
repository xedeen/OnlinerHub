using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Onliner.Resources;
using Onliner.ViewModels;

namespace Onliner
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            //BuildLocalizedApplicationBar();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += MainPage_Loaded;
            MainPanorama.SelectionChanged += PageChanged;
        }

        private void BuildLocalizedApplicationBar()
        {   
            ApplicationBar = new ApplicationBar();
            var appBarButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/1f503-Refresh.48.png", UriKind.Relative));
            appBarButton.Text = AppResources.RefreshMenuText;
            appBarButton.Click += Refresh_OnClick;
            ApplicationBar.Buttons.Add(appBarButton);
        }

        private void PageChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (MainPanorama.SelectedItem as PanoramaItem);
            if (null != item && !App.ViewModel.IsLoading)
            {
                switch (item.Name)
                {
                    case "Tech":
                        App.ViewModel.LoadData(FeedType.Tech);
                        break;
                    case "Auto":
                        App.ViewModel.LoadData(FeedType.Auto);
                        break;
                    case "People":
                        App.ViewModel.LoadData(FeedType.People);
                        break;
                    case "Realt":
                        App.ViewModel.LoadData(FeedType.Realt);
                        break;
                }
            }

        }

        private void OnFeedSelected(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as LongListSelector) == null
                ? null
                : (sender as LongListSelector).SelectedItem as FeedItemViewModel == null
                    ? null
                    : (sender as LongListSelector).SelectedItem;

            if (null == (item as FeedItemViewModel) || null == (item as FeedItemViewModel).Uri) return;
            var uri = (item as FeedItemViewModel).Uri;

            (sender as LongListSelector).SelectedItem = null;
            App.ViewModel.SetFeedSelection(item as FeedItemViewModel);
            NavigationService.Navigate(new Uri(string.Format("/ArticlePage.xaml?uri={0}", uri), UriKind.Relative));
        }


        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPanorama.DefaultItem = MainPanorama.Items[1];
            if (!App.ViewModel.IsLoading)
            {
                App.ViewModel.LoadData(FeedType.Auto);
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }
            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            var binding = new Binding("IsLoading") { Source = DataContext };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = DataContext };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = AppResources.FeedLoadingMsg;
        }


        private void Refresh_OnClick(object sender, EventArgs e)
        {
            App.ViewModel.LoadData(App.ViewModel.CurrentFeedType);
        }
    }
}