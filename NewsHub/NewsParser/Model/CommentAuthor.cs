using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsParser.Model.Base;

namespace NewsParser.Model
{
    public class CommentAuthor : ParagraphBase
    {
        public string ProfileUrl { get; set; }
        public string AvatarSourceUrl { get; set; }
        public string Name { get; set; }
    }
}
