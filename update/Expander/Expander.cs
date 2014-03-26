using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Expander
{
    [TemplateVisualState(Name = "Collapsed", GroupName = "ViewStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "ViewStates")]
    [TemplatePart(Name = "Content", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ExpandCollapseButton", Type = typeof(ToggleButton))]
    public class Expander : ContentControl
    {
        private bool _useTransitions = true;
        private VisualState _collapsedState;
        private ToggleButton _toggleExpander;
        private FrameworkElement _contentElement;

        public static readonly DependencyProperty HeaderContentProperty =
        DependencyProperty.Register("HeaderContent", typeof(object),
        typeof(Expander), null);

        public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register("IsExpanded", typeof(bool),
        typeof(Expander), new PropertyMetadata(true));

        public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
        typeof(Expander), null);

        public object HeaderContent
        {
            get { return GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Expander()
        {
            DefaultStyleKey = typeof(Expander);
        }

        private void ChangeVisualState(bool useTransitions)
        {
            if (IsExpanded)
            {
                if (_contentElement != null)
                {
                    _contentElement.Visibility = Visibility.Visible;
                }
                VisualStateManager.GoToState(this, "Expanded", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Collapsed", useTransitions);
                _collapsedState = (VisualState)GetTemplateChild("Collapsed");
                if (_collapsedState == null)
                {
                    if (_contentElement != null)
                    {
                        _contentElement.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
            _toggleExpander.IsChecked = IsExpanded;
            ChangeVisualState(_useTransitions);
        }

        private void Collapsed_Completed(object sender, EventArgs e)
        {
            _contentElement.Visibility = Visibility.Collapsed;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _toggleExpander = (ToggleButton)GetTemplateChild("ExpandCollapseButton");
            if (_toggleExpander != null)
            {
                _toggleExpander.Click += Toggle_Click;
            }
            _contentElement = (FrameworkElement)GetTemplateChild("Content");
            if (_contentElement != null)
            {
                _collapsedState = (VisualState)GetTemplateChild("Collapsed");
                if ((_collapsedState != null) && (_collapsedState.Storyboard != null))
                {
                    _collapsedState.Storyboard.Completed += Collapsed_Completed;
                }
            }
            ChangeVisualState(false);
        }

    }
}
