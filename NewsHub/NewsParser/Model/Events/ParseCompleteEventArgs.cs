using System;
using System.Collections.Generic;

namespace NewsParser.Model.Events
{
    public delegate void ArticleCompleteDelegate(object sender, ArticleCompleteEventArgs args);
    public delegate void CommentsCompleteDelegate(object sender, CommentsCompleteEventArgs args);

    public class ArticleCompleteEventArgs : EventArgs
    {
        public Article Article { get; set; }
        public bool Success { get; set; }
    }

    public class CommentsCompleteEventArgs : EventArgs
    {
        public List<Comment> Comments { get; set; }
        public bool Success { get; set; }
    }
}
