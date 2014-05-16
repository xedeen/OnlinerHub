using System;
using HAP=HtmlAgilityPack;

namespace NewsParser.Model.Events
{
    public delegate void LoadCompleteDelegate(object sender, LoadCompleteEventArgs args);

    public class LoadCompleteEventArgs : EventArgs
    {
        public HAP.HtmlDocument Page { get; set; }
        public bool Success { get; set; }
    }
}
