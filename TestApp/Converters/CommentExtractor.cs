using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Onliner.OnlinerHub;

namespace Onliner.Converters
{
    public class CommentExtractor :IValueConverter
    {
        public CommentExtractor()
        {
            
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"ContentPanel\"  Margin=\"12,0,12,0\"><Grid.RowDefinitions>";
            

            //var xaml =
              //  "<RichTextBox xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" TextWrapping=\"Wrap\" >";
            if (value != null && value is OnlinerHub.ContentDto)
            {
                var param = (OnlinerHub.ContentDto) value;
                var list = new List<string>();
                foreach (var block in param.items)
                {
                    if (block.is_blockquote)
                        list.AddRange(ProcessBlockQuote(block, 0));
                    else
                    {
                        var content = ProcessBlockContent(block, 0);
                        if (!string.IsNullOrEmpty(content))
                            list.Add(content);
                    }
                }

                for (var i = 0; i < list.Count; ++i)
                    x += "<RowDefinition Height=\"Auto\"/>";
                x += "</Grid.RowDefinitions>";

                for (var i = 0; i < list.Count; ++i)
                {
                    x += list[i].Replace("{ROWNUM}", i.ToString(CultureInfo.InvariantCulture));
                }
                x += "</Grid>";
            }

            return XamlReader.Load(x);
        }

        
        private List<string> ProcessBlockQuote(BlockItemDto block, int level)
        {
            var list = new List<string>();
            if (block == null || !block.is_blockquote)
                return new List<string>();
            if (null != block.children)
            {
                foreach (var child in block.children)
                {
                    list.AddRange(ProcessBlockQuote(child, level + 1));
                }
            }
            var content = ProcessBlockContent(block, level + 1);
            if (!string.IsNullOrEmpty(content))
                list.Add(content);
            return list;
        }
        private string ProcessBlockContent(BlockItemDto block, int level)
        {
            var res = string.Empty;

            
            if (block.is_blockquote)
            {
                res += "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Grid.Row=\"{ROWNUM}\" CornerRadius=\"6\" BorderBrush=\"Gray\" BorderThickness=\"2\"";
                var maxlevel = level > 4 ? 4 : level;
                res += string.Format(" Margin=\"{0},5,0,0\">", 32*maxlevel);
                res += "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Width=\"Auto\" HorizontalAlignment=\"Stretch\"";

                res += " Margin=\"5,0,5,0\" Visibility=\"Visible\">";
                if (!string.IsNullOrEmpty(block.title))
                {
                    res += string.Format("<TextBlock Text=\"{0}\"", block.title);
                    res += " Style=\"{Binding Converter={StaticResource FeedHeaderForegroundSelector}}\" FontWeight=\"Bold\" />";
                }
            }
            else
            {
                res += "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Grid.Row=\"{ROWNUM}\"  Width=\"Auto\" HorizontalAlignment=\"Stretch\">";
            }
            
            res += "<RichTextBox xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Style=\"{Binding Converter={StaticResource ArticleTextForegroundSelector}}\"  TextWrapping=\"Wrap\" TextAlignment=\"Left\">";
            res += "<Paragraph>";
            var isEmpty = true;

            foreach (var item in block.content.items)
            {
                if (item.link != null)
                    res += string.Format("<Hyperlink NavigateUri=\"{0}\" TargetName=\"_blank\">{1} </Hyperlink>",
                        item.link, item.content);
                else
                {
                    if (!string.IsNullOrEmpty(item.content))
                    {
                        res += string.Format("<Run Text=\"{0}\" {1} {2}/>", item.content,
                            item.type.ToLower().Contains("b") ? "FontWeight=\"Bold\"" : string.Empty,
                            item.type.ToLower().Contains("i") ? "FontStyle=\"Italic\"" : string.Empty);
                        isEmpty = false;
                    }
                    else
                    {
                        res += "LineBreak />";
                    }
                }
            }
            res += "</Paragraph></RichTextBox></StackPanel>";
            if (block.is_blockquote)
                res += "</Border>";

            return isEmpty ? string.Empty : res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
