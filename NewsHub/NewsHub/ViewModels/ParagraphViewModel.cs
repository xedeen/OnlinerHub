using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Phone.Tasks;
using NewsHub.Commands;
using NewsParser.Model;
using NewsParser.Model.Base;

namespace NewsHub.ViewModels
{
    public class ParagraphViewModel
    {
        public ParagraphBase Content { get; set; }
        public ICommand LinkCmd { get; private set; }
        public ICommand CommentsCmd { get; set; }
        public ParagraphViewModel()
        {
            LinkCmd = new RelayCommand(ClickHandler);
        }

        private void ClickHandler(object obj)
        {
            var content = (this.Content as P);

            if (null == content) return;
            var img =
                content.Items.FirstOrDefault(i => (i is Image) && !string.IsNullOrEmpty(((Image) i).HRef)) as Image;
            if (null == img) return;
            var webBrowserTask = new WebBrowserTask
            {
                Uri = new Uri(img.HRef, UriKind.Absolute)
            };
            webBrowserTask.Show();
        }



    }
}
