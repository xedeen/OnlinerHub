using System.Collections.Generic;
using NewsParser.Model.Base;

namespace NewsParser.Model
{
    public class P : ParagraphBase
    {
        public List<ContentItemBase> Items { get; set; }
    }
}
