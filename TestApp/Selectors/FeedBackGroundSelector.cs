using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Onliner.Selectors
{
    public sealed class FeedBackGroundSelector : IValueConverter
    {
        public FeedBackGroundSelector()
        {}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var applicationStyle = (string)value;
            if (applicationStyle == "metro")
                return (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
            
            if (applicationStyle == "paper")
                return (Brush)new SolidColorBrush(Color.FromArgb(255, 250, 247, 238));

            if (applicationStyle == "light")
                return (Brush)new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

            if (applicationStyle == "dark")
                return (Brush)new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            
            return (Brush)new SolidColorBrush(Color.FromArgb(255, 250, 247, 238));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
