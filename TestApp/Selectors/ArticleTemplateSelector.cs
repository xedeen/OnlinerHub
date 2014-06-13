using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Onliner.Model;
using Onliner.ViewModels;
using OnlinerHub.Model;

namespace Onliner.Selectors
{
    public class ArticleTemplateSelector : TemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (null != item as ArticleParagraphModel)
            {
                var dt = (DataTemplate) XamlReader.Load((item as ArticleParagraphModel).Xaml);
                return dt;
            }
            return (DataTemplate) XamlReader.Load("<DataTemplate/>");
        }
    }
}
