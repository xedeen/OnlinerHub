using System.Collections.Generic;
using CacheHub.Model.Base;

namespace CacheHub.Model
{
    public class Comment : ParagraphBase
    {
        public CommentAuthor Author { get; set; }
        public List<BlockItem> Content { get; set; }
        public long Id { get; set; }
    }
}
