using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NewsHub.OnlinerHub;
using NewsHub.Resources;
using NewsHub.ViewModels;

namespace NewsHub
{
    public partial class ArticlePage : PhoneApplicationPage
    {
        private int _offsetKnob = 3;
        private int _pageNumber = 0;

        public ArticlePage()
        {
            InitializeComponent();
            DataContext = App.ArticleViewModel;
            this.Loaded += Page_Loaded;
            this.OrientationChanged += Page_OrientationChanged;
            App.ArticleViewModel.PropertyChanged += Article_Changed;

        }

        private void Article_Changed(object sender, PropertyChangedEventArgs e)
        {
            
        }

        private void Page_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
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
            progressIndicator.Text = "Loading"; //AppResources.ArticleLoadingMsg;

        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string uri = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("uri", out uri))
            {   
                App.ArticleViewModel.LoadData(uri);
                _pageNumber++;
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (!App.ArticleViewModel.IsLoading && ContentListBox.ItemsSource != null &&
                ContentListBox.ItemsSource.Count >= _offsetKnob && !App.ArticleViewModel.Article.EOF)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if (null!=e.Container.Content && e.Container.Content is ParagraphBase &&
                        (e.Container.Content as ParagraphBase).Equals(
                            ContentListBox.ItemsSource[ContentListBox.ItemsSource.Count - _offsetKnob]))
                    {
                        App.ArticleViewModel.LoadPage(_pageNumber++);
                    }
                }
            }

        }
    }
}