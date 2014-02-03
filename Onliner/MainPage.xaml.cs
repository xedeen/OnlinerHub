using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Onliner.Controller;
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
            App.ViewModel.FeedsDownloaded += OnDownloadFeeds;

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void OnDownloadFeeds(object sender, FeedsDownloadedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModel.SetItems(e.Items, e.feedType);
                DataContext = null;
                this.State[e.feedType] = DataContext = App.ViewModel;
                progressBar1.Visibility = Visibility.Collapsed;
            });
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var o = pivot1.SelectedIndex = 0;
            RequireFeedsForPage("auto");

        }

        
        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private void OnFeedSelected(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as LongListSelector) == null
                ? null
                : (sender as LongListSelector).SelectedItem as FeedItemViewModel == null
                    ? null
                    : (sender as LongListSelector).SelectedItem;
            
            if (null == (item as FeedItemViewModel) || null == (item as FeedItemViewModel).Link) return;
            var uri = (item as FeedItemViewModel).Link;
            (sender as LongListSelector).SelectedItem = null;
            NavigationService.Navigate(new Uri(string.Format("/ArticleViewerPage.xaml?uri={0}", uri), UriKind.Relative));
        }

        private void RequireFeedsForPage(string pageName)
        {
            if (this.State.ContainsKey(pageName))
            {
                DataContext = null;
                this.State[pageName] = DataContext = App.ViewModel;
            }
            else
            {
                DataContext = null;
                progressBar1.Visibility = Visibility.Visible;
                App.ViewModel.StartRetrieveItems(pageName);
            }
        }

        private void OnPageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (pivot1.SelectedItem as PivotItem);
            if (null != item)
            {
                RequireFeedsForPage(item.Name);
            }
        }
    }
}