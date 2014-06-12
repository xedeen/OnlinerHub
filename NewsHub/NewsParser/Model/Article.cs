using System.Collections.Generic;
using NewsParser.Model.Base;

namespace NewsParser.Model
{
    public class Article
    {
        public long PostId { get; set; }
        public Header Header { get; set; }
        public List<ParagraphBase> Content { get; set; }
    }
}
