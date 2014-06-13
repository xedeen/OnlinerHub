using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Phone.Tasks;
using Onliner.Model.Commands;
using Onliner.OnlinerHub;

namespace Onliner.Model
{
    public class ArticleParagraphModel
    {
        public int InnerId { get; set; }
        public ContentItemType ContentType { get; set; }
        public string Xaml { get; set; }
        public ICommand LinkCmd { get; private set; }
        public string Link { get; set; }

        public ArticleParagraphModel()
        {
            LinkCmd = new RelayCommand(ClickHandler);

        }

        private void ClickHandler(object obj)
        {
            if (string.IsNullOrEmpty(Link))
                return;
          
            var webBrowserTask = new WebBrowserTask
            {
                Uri = new Uri(Link, UriKind.Absolute)
            };
            webBrowserTask.Show();

        }
    }
}
