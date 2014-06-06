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
using Microsoft.Phone.Controls;
using Onliner.ViewModels;

namespace Onliner.Selectors
{
    public sealed class MarginsSelector : IValueConverter
    {
        public MarginsSelector()
        {}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isFeedPage = value is FeedItemViewModel;
            var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;

            return phoneApplicationFrame != null &&
                   (phoneApplicationFrame.Orientation == PageOrientation.LandscapeLeft ||
                    phoneApplicationFrame.Orientation == PageOrientation.LandscapeRight)
                ? (int) 580
                : (int) 400;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
