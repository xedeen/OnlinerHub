using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.AppModel
{
    public class Article : Entity
    {
        public string Title { get; set; }
        public string Uri { get; set; }
        public DateTime? LastUpdate { get; set; }
        public FeedType? FeedType { get; set; }
    }
}
