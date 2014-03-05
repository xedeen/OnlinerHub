using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HAP = HtmlAgilityPack;

namespace OnlinerHub.Model
{
    public class CommentModel
    {
        public int inner_id { get; set; }
        public AuthorModel author { get; set; }
        public ContentModel content { get; set; }

        public static CommentModel ParseComment(HAP.HtmlNode commentNode, int innerId)
        {

            var id = 0;
            Int32.TryParse(commentNode.Attributes["data-comment-id"].Value, out id);
            return new CommentModel
            {
                inner_id = innerId,
                author = AuthorModel.ParseAuthor(commentNode.SelectSingleNode("div/strong[@class='author']")),
                content = ContentModel.ParseContent(commentNode.SelectSingleNode("div[@class='comment-content']"))
            };
        }
    }
}
