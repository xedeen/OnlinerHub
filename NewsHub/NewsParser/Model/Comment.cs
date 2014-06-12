using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsParser.Model.Base;

namespace NewsParser.Model
{
    public class Comment : ParagraphBase
    {
        public CommentAuthor Author { get; set; }
        public List<BlockItem> Content { get; set; }
        public long Id { get; set; }
    }
}
