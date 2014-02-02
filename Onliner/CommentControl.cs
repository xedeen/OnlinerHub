using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Onliner
{
    public class CommentControl : Control
    {
        private Grid _outline;
        private OnlinerHub.CommentDto _comment;

        public CommentControl()
        {
            Template = XamlReader.Load(
                "<ControlTemplate " +
                "xmlns='http://schemas.microsoft.com/client/2007' " +
                "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>" +
                "<Grid " +
                "x:Name=\"OutlineGrid\" " +
                "/>" +
                "</ControlTemplate>") as ControlTemplate;
            ApplyTemplate();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _outline = GetTemplateChild("OutlineGrid") as Grid;
        }


        protected void ParseAndSetGrid(OnlinerHub.CommentDto comment)
        {   
            _comment = comment;

            foreach (var block in comment.content.items)
            {
                var stackPanel = new StackPanel();

                if (block.is_blockquote)
                {
                    stackPanel.Margin = new Thickness(32, 0, 0, 0);
                    stackPanel.Background = ((SolidColorBrush) Resources["PhoneInactiveBrush"]);
                    
                }

                _outline.RowDefinitions.Add(new RowDefinition{Height =GridLength.Auto});
            }
            
        }
    }
}
