using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using NewsHub.ViewModels;
using NewsParser.Model;

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
                "<RichTextBox TextWrapping=\"Wrap\" Style=\"{StaticResource NormalTextArticle}\">");
            sb.AppendLine("<Paragraph TextAlignment=\"Justify\">");

            var isEmpty = true;

            var formatStart = "<Grid>";
            var videoFormatEnd =
                "<Button  Command=\"{Binding LinkCmd}\" BorderThickness=\"0\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\">" +
                "<StackPanel>" +
                "<Image Source=\"Assets\\Video.png\" Height=\"128\" Width=\"128\" />" +
                "</StackPanel>" +
                "</Button>" +
                "</Grid>";
            var imgFormatEnd =
                "<Button  Command=\"{Binding LinkCmd}\" BorderThickness=\"0\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\">" +
                "<StackPanel>" +
                "<Image Source=\"Assets\\ImgLink.png\" Height=\"128\" Width=\"128\" />" +
                "</StackPanel>" +
                "</Button>" +
                "</Grid>";

            foreach (var item in p.Items)
            {
                if (item is A)
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        sb.Append("<Hyperlink Foreground=\"{StaticResource PhoneAccentBrush}\" NavigateUri=\"");
                        sb.Append((item as A).HRef);
                        sb.Append("\" TargetName=\"_blank\">");
                        sb.Append(item.Text);
                        sb.Append("</Hyperlink>");
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
                    sb.AppendLine("<InlineUIContainer>");
                    if (!string.IsNullOrEmpty((item as Image).HRef))
                        sb.AppendLine(formatStart);
                    sb.AppendLine("<Image VerticalAlignment=\"Stretch\"><Image.Source>");

                    sb.AppendLine(string.Format(
                        "<BitmapImage UriSource=\"{0}\" CreateOptions=\"BackgroundCreation\"/></Image.Source></Image>",
                        (item as Image).SourceUrl));

                    if (item is Video && !string.IsNullOrEmpty((item as Video).HRef))
                        sb.AppendLine(videoFormatEnd);

                    else if (!string.IsNullOrEmpty((item as Image).HRef))
                        sb.AppendLine(imgFormatEnd);

                    sb.AppendLine("</InlineUIContainer>");
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

            if (null != item as ParagraphViewModel && null!=(item as ParagraphViewModel).Content)
            {
                var content = (item as ParagraphViewModel).Content;
                if (content is P)
                    x += ProcessParagraph(content as P);
                if (content is Table)
                    x += ProcessTable(content as Table);
            }
            x += "</Grid> </DataTemplate>";
            var dt = (DataTemplate)XamlReader.Load(x);
            return dt;
        }
    }
}
