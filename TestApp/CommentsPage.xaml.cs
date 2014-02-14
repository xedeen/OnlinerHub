using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Onliner.Model;
using Onliner.Resources;
using Onliner.ViewModels;

namespace Onliner
{
    public partial class CommentsPage : PhoneApplicationPage
    {
        private int _pageNumber = 0;
        private CommentsViewModel _viewModel;
        private int _offsetKnob = 7;


        public CommentsPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            _viewModel = (CommentsViewModel)Resources["commentsViewModel"];
            commentsListBox.ItemRealized += commentsListBox_ItemRealized;
            this.Loaded += CommentsPage_Loaded;
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            var appBarButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/1f503-Refresh.48.png", UriKind.Relative));
            appBarButton.Text = AppResources.RefreshMenuText;
            appBarButton.Click += Refresh_OnClick;
            ApplicationBar.Buttons.Add(appBarButton);
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
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
                _viewModel.LoadCommentPage(_pageNumber++);
            }
            else
            {
                NavigationService.GoBack();
            }
        }


        private void CommentsPage_Loaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }
            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            var binding = new Binding("IsLoading") {Source = _viewModel};
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") {Source = _viewModel};
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = AppResources.CommentLoadingMsg;
        }

        private void commentsListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (!_viewModel.IsLoading && commentsListBox.ItemsSource != null &&
                commentsListBox.ItemsSource.Count >= _offsetKnob && !_viewModel.EOF)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if (
                        (e.Container.Content as CommentModel).Equals(
                            commentsListBox.ItemsSource[commentsListBox.ItemsSource.Count - _offsetKnob]))
                    {
                        _viewModel.LoadCommentPage(_pageNumber++);
                    }
                }
            }

        }

        private void Refresh_OnClick(object sender, EventArgs e)
        {
            _pageNumber = 0;
            _viewModel.LoadCommentPage(_pageNumber++);
        }
    }
}