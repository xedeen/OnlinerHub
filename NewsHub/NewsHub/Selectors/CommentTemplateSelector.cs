using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using NewsHub.ViewModels;
using NewsParser.Model;

namespace NewsHub.Selectors
{
    public sealed class CommentTemplateSelector : TemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var x = @"<DataTemplate
                            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">";



            if (null != item as Comment && null != (item as Comment).Content && null != (item as Comment).Author)
            {
                x += ProcessComment(item as Comment);
            }
            x += "</Grid> </DataTemplate>";
            var dt = (DataTemplate) XamlReader.Load(x);
            return dt;
        }

        private string ProcessComment(Comment comment)
        {
            var x = "<Grid Name=\"ContentPanel\"  Margin=\"12,0,12,0\">";
            x += "<Grid.RowDefinitions>";
            x += "<RowDefinition Height=\"Auto\"/>";
            x += "<RowDefinition Height=\"*\"/>";
            x += "</Grid.RowDefinitions>";
            x+=ProcessAuthor(comment.Author);
            x+="<Grid Grid.Row=\"1\">";

            x += "</Grid>";
            return x;
        }

        private string ProcessAuthor(CommentAuthor author)
        {
            var x = "<Grid Background=\"{StaticResource PhoneChromeBrush}\">";
            x += "<Grid.ColumnDefinitions>";
            x += "<ColumnDefinition Width=\"Auto\"/>";
            x += "<ColumnDefinition Width=\"*\"/>";
            x += "</Grid.ColumnDefinitions>";

            x += "<Image Height=\"50\" Width=\"50\" Grid.Column=\"0\" VerticalAlignment=\"Top\" Margin=\"0,7,7,0\">";
            x += "<Image.Source>";
            x += string.Format("<BitmapImage UriSource=\"{0}\" CreateOptions=\"BackgroundCreation\"/>",author.AvatarSourceUrl);
            x += "</Image.Source></Image>";


            x += string.Format("<TextBlock Text=\"{0}\" Grid.Column=\"1\"", author.Name);
            x += " Foreground=\"{StaticResource PhoneAccentBrush}\" VerticalAlignment=\"Top\"/></Grid>";
            return x;
        }
    }
}
