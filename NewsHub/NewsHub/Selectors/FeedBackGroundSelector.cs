using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NewsHub.Selectors
{
    public sealed class FeedBackGroundSelector : IValueConverter
    {
        public FeedBackGroundSelector()
        {}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isClearTemplate = (bool)value;
            if (!isClearTemplate && false)
            {
                var lightThemeEnabled = (Visibility)Application.Current.Resources
                        ["PhoneLightThemeVisibility"] == Visibility.Visible;

                var url = lightThemeEnabled
                    ? "/Onliner;component/Assets/ocean_lights_light.png"
                    : "/Onliner;component/Assets/leather.png";
                var brush = new ImageBrush { ImageSource = new BitmapImage(new Uri(url, UriKind.Relative)) };
                return brush;
            }
            //return (Brush) Application.Current.Resources["PhoneBackgroundBrush"];
            //return (Brush) Application.Current.Resources["PhoneForegroundBrush"];
            return (Brush)new SolidColorBrush(Color.FromArgb(255, 250, 247, 238));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
