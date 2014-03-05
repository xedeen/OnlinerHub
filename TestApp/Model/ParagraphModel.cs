using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinerHub.Model
{
    public class ParagraphModel
    {   
        public List<ParagraphItemModel> items { get; set; }

        public ParagraphModel()
        {
            items = new List<ParagraphItemModel>();
        }
    }
}
