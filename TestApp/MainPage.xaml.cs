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
            BuildLocalizedApplicationBar();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += MainPage_Loaded;
        }

        private void BuildLocalizedApplicationBar()
        {   
            ApplicationBar = new ApplicationBar();
            var menuItem = new ApplicationBarMenuItem(AppResources.SettingsTitle);
            menuItem.Click += Settings_OnClick;
            ApplicationBar.MenuItems.Add(menuItem);
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
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

        private void OnFeedSelected(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as LongListSelector) == null
                ? null
                : (sender as LongListSelector).SelectedItem as FeedItemViewModel == null
                    ? null
                    : (sender as LongListSelector).SelectedItem;

            if (null == (item as FeedItemViewModel) || null == (item as FeedItemViewModel).Uri) return;
            var uri = (item as FeedItemViewModel).Uri;
            var title = (item as FeedItemViewModel).TitleU;

            (sender as LongListSelector).SelectedItem = null;
            App.ViewModel.SetFeedSelection(item as FeedItemViewModel);
            NavigationService.Navigate(new Uri(string.Format("/ArticleViewPage.xaml?uri={0}&title={1}", uri, title),
                UriKind.Relative));
        }


        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var items = new List<PivotItem>{Tech, Auto, People, Realt};
            FeedSourcePivot.Items.Clear();
            
            foreach (var pi in items)
            {
                if (pi.Visibility==Visibility.Visible)
                    FeedSourcePivot.Items.Add(pi);
            }
            if (FeedSourcePivot.Items.Count==0)
                return;

            var pageN = (int) App.ViewModel.CurrentFeedType;
            if (items[pageN].Visibility == Visibility.Visible)
            {
                FeedSourcePivot.SelectedItem = items[pageN];
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Visibility == Visibility.Visible)
                    {
                        FeedSourcePivot.SelectedItem = items[i];
                        App.ViewModel.CurrentFeedType = (FeedType) i;
                    }
                }
            }
            if (!App.ViewModel.IsLoading)
            {
                App.ViewModel.LoadData(App.ViewModel.CurrentFeedType);
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

        private void Settings_OnClick(object sender, EventArgs e)
        {   
            NavigationService.Navigate(
                new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}