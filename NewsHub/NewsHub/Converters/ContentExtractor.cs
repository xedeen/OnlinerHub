using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using NewsHub.OnlinerHub;

namespace NewsHub.Converters
{
    public class ContentExtractor :IValueConverter
    {
        public ContentExtractor()
        {
            
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"ContentPanel\"  Margin=\"12,0,12,0\">";
            if (null != value)
            {
                if (value is P)
                    x += ProcessParagraph(value as P);
                if (value is Table)
                    x += ProcessTable(value as Table);
            }
            x += "</Grid>";
            return XamlReader.Load(x);
        }

        private string ProcessTable(Table table)
        {
            return string.Empty;
        }

        private string ProcessParagraph(P p)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Width=\"Auto\" HorizontalAlignment=\"Stretch\">");
            sb.AppendLine(
                "<RichTextBox xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" TextWrapping=\"Wrap\" TextAlignment=\"Left\">");
            sb.AppendLine("<Paragraph>");

            var isEmpty = true;
            foreach (var item in p.Items)
            {
                if (item is A)
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        sb.AppendLine(
                            string.Format("<Hyperlink NavigateUri=\"{0}\" TargetName=\"_blank\">{1} </Hyperlink>",
                                (item as A).HRef, item.Text));
                        isEmpty = false;
                    }
                if (item is TextBlock)
                {
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var modifiers = (item as TextBlock).TextModifiers;
                        sb.AppendLine(
                            string.Format("<Run Text=\"{0}\" {1} {2}/>", item.Text,
                                (modifiers | 0x0001) == modifiers ? "FontWeight=\"Bold\"" : string.Empty,
                                (modifiers | 0x0010) == modifiers ? "FontStyle=\"Italic\"" : string.Empty));
                        isEmpty = false;
                    }
                }
                if (item is Image)
                {
                    if (item is Video)
                    {
                        
                    }
                    else
                    {
                        sb.AppendLine(string.Format(
                            "<BlockUIContainer xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Image  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Source=\"{0}\"/></BlockUIContainer>",
                            (item as Image).SourceUrl));
                        isEmpty = false;
                    }
                }
            }
            sb.AppendLine("</Paragraph></RichTextBox></StackPanel>");
            return isEmpty ? string.Empty : sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
