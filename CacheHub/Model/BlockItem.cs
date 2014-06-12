using System.Collections.Generic;

namespace CacheHub.Model
{
    public class BlockItem
    {
        public bool  IsBlockquote { get; set; }
        public string Title { get; set; }
        public List<BlockItem> Children { get; set; }
        public P Content { get; set; }
    }
}
