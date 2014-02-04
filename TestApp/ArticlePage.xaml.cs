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
using TestApp.ViewModels;

namespace TestApp
{
    public partial class ArticlePage : PhoneApplicationPage
    {
        // Constructor
        public ArticlePage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += MainPage_Loaded;
            App.ViewModel.Article.PropertyChanged += Article_PropertyChanged;
        }

        void Article_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content" && !string.IsNullOrEmpty(App.ViewModel.Article.Content))
            {
                Browser.NavigateToString(App.ViewModel.Article.Content);
            }
        }

        private void Browser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            Browser.Opacity = 1;
        }


        private void PageChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Browser.Opacity = 0;

            string uri = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("uri", out uri))
            {
                App.ViewModel.Article.Uri = uri;
                App.ViewModel.LoadArticle(uri);
            }
            else
            {
                NavigationService.GoBack();
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

            progressIndicator.Text = "Loading article...";

        }


        private void Comments_OnClick(object sender, EventArgs e)
        {
            var uri = App.ViewModel.Article.Uri;
            NavigationService.Navigate(
                new Uri(string.Format("/CommentsPage.xaml?uri={0}&title={1}", uri, App.ViewModel.Article.Title),
                    UriKind.Relative));
        }
    }
}