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
            _viewModel = (ArticleContentViewModel)Resources["contentViewModel"];
            this.Loaded += ArticleViewPage_Loaded;
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