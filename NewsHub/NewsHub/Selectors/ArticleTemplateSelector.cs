using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using NewsHub.OnlinerHub;

namespace NewsHub.Selectors
{
    public sealed class ArticleTemplateSelector : TemplateSelector
    {
        private string ProcessTable(Table table)
        {
            return string.Empty;
        }

        private string ProcessParagraph(P p)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                "<StackPanel Width=\"Auto\" HorizontalAlignment=\"Stretch\" Margin=\"0,10,0,10\">");
            sb.AppendLine(
                "<RichTextBox TextWrapping=\"Wrap\">");
            sb.AppendLine("<Paragraph TextAlignment=\"Justify\">");

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
                    sb.AppendLine(
                        "<InlineUIContainer>" +
                        (!string.IsNullOrEmpty((item as Image).HRef) ? "<Button Command=\"{Binding Path=ItemClickCommand}\">" : "") +
                        "<Image VerticalAlignment=\"Stretch\">" +
                        "<Image.Source>"
                        );
                    
                    sb.AppendLine(string.Format(
                        "<BitmapImage UriSource=\"{0}\" CreateOptions=\"BackgroundCreation\"/>" +
                        "</Image.Source></Image>" +
                        (!string.IsNullOrEmpty((item as Image).HRef) ? "</Button>" : "") +
                        "</InlineUIContainer>",
                        (item as Image).SourceUrl));
                    isEmpty = false;
                }
            }
            sb.AppendLine("</Paragraph></RichTextBox></StackPanel>");
            return isEmpty ? string.Empty : sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var x = @"<DataTemplate
                            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">";
            x += "<Grid Name=\"ContentPanel\"  Margin=\"12,0,12,0\">";

            if (null != item)
            {
                if (item is P)
                    x += ProcessParagraph(item as P);
                if (item is Table)
                    x += ProcessTable(item as Table);
            }
            x += "</Grid> </DataTemplate>";
            var dt = (DataTemplate)XamlReader.Load(x);
            return dt;
        }
    }
}
