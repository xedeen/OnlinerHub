using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using Onliner.OnlinerHub;

namespace Onliner.Controller
{
    public class CommentExtractor :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var xaml =
                "<RichTextBox xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" TextWrapping=\"Wrap\" >";
            if (value != null && value is OnlinerHub.ContentDto)
            {
                var param = (OnlinerHub.ContentDto) value;

                bool isEmpty;
                
                foreach (var block in param.items)
                {
                    string blockXaml = ProcessBlock(block, out isEmpty);
                    if (!isEmpty)
                    {

                        xaml += blockXaml;
                    }
                }
                xaml += "</RichTextBox>";
            }
            return XamlReader.Load(xaml);
        }

        private string ProcessBlock(BlockItemDto block, out bool isEmpty)
        {
            var result = string.Format("<Paragraph {0}>", string.Empty);
                //block.is_blockquote ? "Background=\"{StaticResource PhoneInactiveBrush}\"" : string.Empty);
            isEmpty = true;
            foreach (var item in block.content.items)
            {
                if (item.link != null)
                    result += string.Format("<Hyperlink NavigateUri=\"{0}\">{1}</Hyperlink>", item.link, item.content);
                else
                {
                    if (!string.IsNullOrEmpty(item.content))
                    {
                        result += string.Format("<Run Text=\"{0}\" {1} {2}/>", item.content,
                            item.type.ToLower().Contains("b") ? "FontWeight=\"Bold\"" : string.Empty,
                            item.type.ToLower().Contains("i") ? "FontStyle=\"Italic\"" : string.Empty);
                        isEmpty = false;
                    }
                    else
                    {
                        result += "LineBreak />";
                    }
                }
            }
            return result + "</Paragraph>";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
