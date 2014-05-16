using System.Collections.Generic;
using NewsParser.Model.Base;

namespace NewsParser.Model
{
    public class Table :ParagraphBase
    {
        public List<TableRow> Rows { get; set; }
    }
}
