using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Onliner.Converters
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
            return (Brush) Application.Current.Resources["PhoneBackgroundBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
