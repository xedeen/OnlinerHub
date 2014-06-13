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
using Onliner.Resources;
using Onliner.ViewModels;

namespace Onliner
{
    public partial class ArticleViewPage : PhoneApplicationPage
    {
        private ArticleContentViewModel _viewModel;

        public ArticleViewPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            _viewModel = (ArticleContentViewModel)Resources["contentViewModel"];
            this.Loaded += ArticleViewPage_Loaded;
            this.OrientationChanged += Page_OrientationChanged;
        }

        private void Page_OrientationChanged(object sender, OrientationChangedEventArgs e)
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

        private void Comments_OnClick(object sender, EventArgs e)
        {
            var uri = _viewModel.Uri;
            NavigationService.Navigate(
                new Uri(string.Format("/CommentsPage.xaml?uri={0}&title={1}", uri, _viewModel.Title),
                    UriKind.Relative));
        }

        private void Refresh_OnClick(object sender, EventArgs e)
        {
            _viewModel.LoadContentPage(0);
        }

        void ArticleViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }
            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            var binding = new Binding("IsLoading") { Source = _viewModel };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = _viewModel };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = AppResources.ArticleLoadingMsg;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string uri = string.Empty;
            string title = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("uri", out uri))
            {
                _viewModel.Uri = new Uri(uri);
                if (NavigationContext.QueryString.TryGetValue("title", out title))
                    _viewModel.Title = title;
                _viewModel.LoadContentPage(0);
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}