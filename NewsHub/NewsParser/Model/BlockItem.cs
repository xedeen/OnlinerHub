using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsParser.Model
{
    public class BlockItem
    {
        public bool  IsBlockquote { get; set; }
        public string Title { get; set; }
        public List<BlockItem> Children { get; set; }
        public P Content { get; set; }
    }
}
