﻿using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Onliner.Resources;

namespace Onliner
{
    public partial class ArticlePage : PhoneApplicationPage
    {
        // Constructor
        public ArticlePage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += MainPage_Loaded;
            this.OrientationChanged += ArticlePage_OrientationChanged;
            App.ViewModel.Article.PropertyChanged += Article_PropertyChanged;
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            var appBarButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/1f503-Refresh.48.png", UriKind.Relative));
            appBarButton.Text = AppResources.RefreshMenuText;
            appBarButton.Click += Refresh_OnClick;
            ApplicationBar.Buttons.Add(appBarButton);
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/e25c-Typing.48.png", UriKind.Relative));
            appBarButton.Text = AppResources.CommentsMenuText;
            appBarButton.Click += Comments_OnClick;
            ApplicationBar.Buttons.Add(appBarButton);
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            
        }

        private void ArticlePage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                LayoutRoot.Margin = new Thickness(0, 0, 0, 0);
            }
            else if ((e.Orientation & PageOrientation.LandscapeLeft) == (PageOrientation.LandscapeLeft))
            {
                LayoutRoot.Margin = new Thickness(0, 0, 72, 0);
            }
            else if ((e.Orientation & PageOrientation.LandscapeRight) == (PageOrientation.LandscapeRight))
            {
                LayoutRoot.Margin = new Thickness(72, 0, 0, 0);
            } 
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
            progressIndicator.Text = AppResources.ArticleLoadingMsg;
        }


        private void Comments_OnClick(object sender, EventArgs e)
        {
            var uri = App.ViewModel.Article.Uri;
            NavigationService.Navigate(
                new Uri(string.Format("/CommentsPage.xaml?uri={0}&title={1}", uri, App.ViewModel.Article.Title),
                    UriKind.Relative));
        }

        private void Refresh_OnClick(object sender, EventArgs e)
        {
            App.ViewModel.LoadArticle(App.ViewModel.Article.Uri);
        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            var vm = App.ViewModel.Article;
            if (vm != null)
            {   
                // User flicked towards left
                if (e.HorizontalVelocity < -1200)
                {
                    var uri = App.ViewModel.GetNextUri();
                    if (!string.IsNullOrEmpty(uri))
                    {
                        Browser.Opacity = 0;
                        App.ViewModel.Article.Uri = uri;
                        App.ViewModel.LoadArticle(uri);
                    }
                    else
                    {
                        var toast = GetToastWithImgAndTitle("It is a last article in list");
                        toast.TextWrapping = TextWrapping.Wrap;
                        toast.Show();

                    }
                }

                // User flicked towards right
                if (e.HorizontalVelocity > 1200)
                {
                    var uri = App.ViewModel.GetPrevUri();
                    if (!string.IsNullOrEmpty(uri))
                    {
                        Browser.Opacity = 0;
                        App.ViewModel.Article.Uri = uri;
                        App.ViewModel.LoadArticle(uri);
                    }
                    else
                    {
                        var toast = GetToastWithImgAndTitle("It is a first article in list");
                        toast.TextWrapping = TextWrapping.Wrap;
                        toast.Show();
                    }
                }
            }
        }

        private static ToastPrompt GetToastWithImgAndTitle(string message)
        {
            return new ToastPrompt
            {
                Title = "Onliner Hub",
                TextOrientation = System.Windows.Controls.Orientation.Vertical,
                Message = message,
            };
        }
    }
}