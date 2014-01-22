using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.AppModel
{
    public class Comment : Entity
    {
        public long ArticleId { get; set; }
        public long AuthorId { get; set; }
        public string CommentContent { get; set; }
    }
}
