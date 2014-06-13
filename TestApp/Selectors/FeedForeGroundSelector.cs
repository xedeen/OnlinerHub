using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
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

    public sealed class ArticleTextForegroundSelector : IValueConverter
    {
        public ArticleTextForegroundSelector()
        { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var applicationStyle = App.ViewModel.Settings.ApplicationStyle;
            var articleFont = App.ViewModel.Settings.ArticleFont;
            var articleFontSize = App.ViewModel.Settings.ArticleFontSize;

            string xaml = @"
<ResourceDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
            xmlns:dxg='http://schemas.devexpress.com/winfx/2008/xaml/grid'
            xmlns:dxgt='http://schemas.devexpress.com/winfx/2008/xaml/grid/themekeys'>
        <Style x:Key='ArticleText' TargetType='RichTextBox'>
            <Setter Property='FontFamily' Value='{0}' />
            <Setter Property='FontSize' Value='{1}' />
            <Setter Property='Foreground' Value='{2}'></Setter>
        </Style>    
</ResourceDictionary>";

            var size = "26.67";
            switch (articleFontSize)
            {
                case "x-small":
                    size = "18.67";
                    break;
                case "small":
                    size="21.33";
                    break;
                case "large":
                case "x-large":
                size="34.67";
                    break;
            }
            var foreGround = "{StaticResource PhoneForegroundBrush}";
            switch (applicationStyle)
            {
                case "paper":
                    foreGround = "#FF0A0A0A";
                    break;
                case "light":
                    foreGround = "#FF000000";
                    break;
                case "dark":
                    foreGround = "#FFFFFFFF";
                    break;
            }
            xaml = string.Format(xaml, articleFont, size, foreGround);
            /*var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream))
            {
                writer.Write(xaml);
                writer.Flush();
                memoryStream.Position = 0;
            }*/
            var dictionary = XamlReader.Load(xaml) as ResourceDictionary;
            return (Style) dictionary["ArticleText"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
