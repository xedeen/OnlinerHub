using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CacheHub.Model
{
    public class FeedCard
    {
        public string Title { get; set; }
        public string Rubric { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public Uri ImageSource  { get; set; }
        public string Description { get; set; }
    }
}