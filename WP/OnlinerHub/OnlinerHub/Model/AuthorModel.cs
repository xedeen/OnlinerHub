using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HAP = HtmlAgilityPack;

namespace OnlinerHub.Model
{
    public class AuthorModel
    {
        public string name { get; set; }
        public string profile_uri { get; set; }
        public string avatar_source_uri { get; set; }

        public static AuthorModel ParseAuthor(HAP.HtmlNode node)
        {
            if (null == node) return null;
            var link = node.SelectSingleNode("a[@href]");
            var img = node.SelectSingleNode("a/figure[@class='author-image']/img[@src]");
            return new AuthorModel
            {
                profile_uri = link == null ? string.Empty : link.Attributes["href"].Value,
                avatar_source_uri = img == null ? string.Empty : img.Attributes["src"].Value,
                name = null == link ? string.Empty : link.InnerText
            };
        }
    }
}
