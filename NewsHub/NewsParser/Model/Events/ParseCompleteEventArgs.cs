using System;

namespace NewsParser.Model.Events
{
    public delegate void ParseCompleteDelegate(object sender, ParseCompleteEventArgs args);

    public class ParseCompleteEventArgs : EventArgs
    {
        public Article Article { get; set; }
        public bool Success { get; set; }
    }
}
