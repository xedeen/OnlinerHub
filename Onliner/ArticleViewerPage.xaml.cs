using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using Windows.System;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Onliner.Model;
using Onliner.ViewModels;

namespace Onliner
{
    public partial class ArticleViewerPage : PhoneApplicationPage
    {
        private int _pageNumber = 0;
        private CommentsViewModel _viewModel;
        private int _offsetKnob = 7;


        public ArticleViewerPage()
        {
            InitializeComponent();
            _viewModel = (CommentsViewModel) Resources["commentsViewModel"];
            commentsListBox.ItemRealized += commentsListBox_ItemRealized;
            this.Loaded += ArticleViewerPage_Loaded;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string uri = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("uri", out uri))
            {
                _viewModel.Uri = new Uri(uri);
                _viewModel.LoadCommentPage(_pageNumber++);
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        private void ArticleViewerPage_Loaded(object sender, RoutedEventArgs e)
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

            progressIndicator.Text = "Loading comments...";

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
                        Debug.WriteLine("Searching for {0}", _pageNumber);
                        _viewModel.LoadCommentPage(_pageNumber++);
                    }
                }
            }
        }

        private void UIElement_OnTap(object sender, GestureEventArgs e)
        {
            var richTB = sender as RichTextBox;
            var textPointer = richTB.GetPositionFromPoint(e.GetPosition(richTB));

            var element = textPointer.Parent as TextElement;
            while (element != null && !(element is Underline))
            {
                if (element.ContentStart != null
                    && element != element.ElementStart.Parent)
                {
                    element = element.ElementStart.Parent as TextElement;
                }
                else
                {
                    element = null;
                }
            }

            if (element == null) return;

            var underline = element as Underline;
            if (underline.Name == "LinkToInfiniteSquare")
            {
                //Launcher.LaunchUriAsync(new Uri("http://www.infinitesquare.com"));
            }
            else if (underline.Name == "LinkToMyBlog")
            {
                //Launcher.LaunchUriAsync(new Uri("http://www.jonathanantoine.com"));
            }
        }
    }
}