using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CacheHub
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOnlinerHub" in both code and config file together.
    [ServiceContract]
    public interface IOnlinerHub
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "uri/{articleUrl}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        string GetReadability(string articleUrl);

        [OperationContract]
        [WebInvoke(UriTemplate = "comments/{articleUrl}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        CommentsPageDto GetComments(string articleUrl, int cursor);

        [OperationContract]
        [WebInvoke(UriTemplate = "content/{articleUrl}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        void GetContent(string articleUrl);
    }

    [DataContract]
    public class ArticleDto
    {
        public List<ArticleParagraphDto> Paragraphs { get; set; }
    }

    [DataContract]
    public class ArticleParagraphDto
    {
        public List<ArticleParagraphContentDto> Content { get; set; }
    }

    [DataContract]
    public class ArticleParagraphContentDto
    {
        public string ContentType { get; set; } //t for text, a for link, i for em b for strong, v for iframe
        public string Content { get; set; }
        public string Url { get; set; } //null for non-link or image source or iframe source
        public string Thumbnail { get; set; } //only for youtube video
        public string VideoId { get; set; }
        public List<ArticleParagraphContentDto> ChildContent { get; set; }
        
    }


    [DataContract]
    public class CommentsPageDto
    {
        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public List<CommentDto> comments { get; set; }

        [DataMember]
        public int? previous_page_cursor { get; set; }

        [DataMember]
        public int? next_page_cursor { get; set; }
    }

    [DataContract]
    public class CommentDto
    {

        public int inner_id { get; set; }

        [DataMember]
        public AuthorDto author { get; set; }

        [DataMember]
        public ContentDto content { get; set; }
    }

    [DataContract]
    public class AuthorDto
    {
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string profile_uri { get; set; }

        [DataMember]
        public string avatar_source_uri { get; set; }
    }

    [DataContract]
    public class ContentDto
    {
        [DataMember]
        public List<BlockItemDto> items { get; set; }

        public ContentDto()
        {
            //paragraph_list = new List<ParagraphDto>();
            items = new List<BlockItemDto>();
        }
    }

    [DataContract]
    public class BlockItemDto
    {
        [DataMember]
        public bool is_blockquote { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public List<BlockItemDto> children { get; set; }

        [DataMember]
        public ParagraphDto content { get; set; }

        public BlockItemDto()
        {
            content = new ParagraphDto();
        }
    }

    [DataContract]
    public class ParagraphDto
    {
        [DataMember]
        public List<ParagraphItem> items { get; set; }

        public ParagraphDto()
        {
            items = new List<ParagraphItem>();
        }
    }

    [DataContract]
    public class ParagraphItem
    {
        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string link { get; set; }

        [DataMember]
        public string content { get; set; }
    }
}
