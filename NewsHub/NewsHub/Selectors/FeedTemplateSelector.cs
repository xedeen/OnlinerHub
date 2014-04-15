using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NewsHub.Selectors
{
    public sealed class FeedTemplateSelector : IValueConverter
    {
        public FeedTemplateSelector()
        {
            
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isClearTemplate = (bool)value;
            if (isClearTemplate)
            {
                return (DataTemplate)Application.Current.Resources["FeedItemTemplateClear"];
            }
            return (DataTemplate)Application.Current.Resources["FeedItemTemplateRich"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
