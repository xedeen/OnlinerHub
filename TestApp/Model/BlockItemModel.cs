using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinerHub.Model
{
    public class BlockItemModel
    {   
        public bool is_blockquote { get; set; }
        public string title { get; set; }
        public List<BlockItemModel> children { get; set; }
        public ParagraphModel content { get; set; }

        public BlockItemModel()
        {
            content = new ParagraphModel();
        }
    }
}
