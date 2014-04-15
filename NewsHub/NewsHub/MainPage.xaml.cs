using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NewsHub.Resources;
using NewsHub.ViewModels;

namespace NewsHub
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += MainPage_Loaded;


            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
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

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var pageN = 1;
            switch (App.ViewModel.CurrentFeedType)
            {
                case FeedType.People:
                    pageN = 2;
                    break;
                case FeedType.Realt:
                    pageN = 3;
                    break;
                case FeedType.Tech:
                    pageN = 0;
                    break;
            }
            FeedSourcePivot.SelectedItem = FeedSourcePivot.Items[pageN];
            if (!App.ViewModel.IsLoading)
            {
                App.ViewModel.LoadData(App.ViewModel.CurrentFeedType);
            }
        }

        private void PageChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (FeedSourcePivot.SelectedItem as PivotItem);
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
            
        }
    }
}