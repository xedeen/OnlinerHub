using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Onliner.Selectors
{
    public class FeedHeaderForegroundSelector :  IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var applicationStyle = App.ViewModel.Settings.ApplicationStyle;

            if (applicationStyle == "paper")
                return (Style)Application.Current.Resources["H1_Paper"];

            if (applicationStyle == "light")
                return (Style)Application.Current.Resources["H1_Light"];

            if (applicationStyle == "dark")
                return (Style)Application.Current.Resources["H1_Dark"];
            else
                return (Style)Application.Current.Resources["H1_Metro"];
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class FeedTextForegroundSelector : IValueConverter
    {
        public FeedTextForegroundSelector()
        { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var applicationStyle = App.ViewModel.Settings.ApplicationStyle;

            if (applicationStyle == "paper")
                return (Style)Application.Current.Resources["Normal_Paper"];

            if (applicationStyle == "light")
                return (Style)Application.Current.Resources["Normal_Light"];

            if (applicationStyle == "dark")
                return (Style)Application.Current.Resources["Normal_Dark"];

            return (Style)Application.Current.Resources["Normal_Metro"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
