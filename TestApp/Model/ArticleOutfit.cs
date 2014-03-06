using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model
{
    public class ArticleOutfit
    {
        public int CommentsCount { get; set; }
        public List<ArticleTag> Tags { get; set; }
        public string PublishTime { get; set; }
        public long ReplyId { get; set; }
        public string Content { get; set; }
    }

    public class ArticleTag
    {
        public string Category { get; set; }
        public string Content { get; set; }
        public string Uri { get; set; }
        public ArticleTagType TagType { get; set; }
    }

    public enum ArticleTagType
    {
        Strong,
        Small
    }

    public class Mappings
    {
        
    }

    
    public class Mapping
    {
        public string DestinationName  { get; set; }
        public MappingType Type { get; set; }
        public string IncludeNodeTypes { get; set; }
        public string ExcludeNodeTypes { get; set; }
        public string XPath { get; set; }
        public string ContentXPath { get; set; }
        public string CategoryXPath { get; set; }
        public string LinkXPath { get; set; }
    }

    public enum MappingType
    {
        SingleNodeContent,
        NodeCollectionContent,
        NodeValue,
        AttributeValue,
        Tagging
    }
}
