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
        [DataMember]
        public CiteDto blockquote { get; set; }
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
    public class CiteDto
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public ContentDto content { get; set; }
        [DataMember]
        public CiteDto child { get; set; }
    }

    [DataContract]
    public class ContentDto
    {
        [DataMember]
        public List<ParagraphDto> paragraph_list { get; set; }

        public ContentDto()
        {
            paragraph_list = new List<ParagraphDto>();
        }
    }

    [DataContract]
    public class ParagraphDto
    {
        [DataMember]
        public List<TextItemDto> items { get; set; }

        public ParagraphDto()
        {
            items = new List<TextItemDto>();
        }
    }

    [DataContract]
    public class TextItemDto
    {
        [DataMember]
        public string content { get; set; }

        [DataMember]
        public Uri link_uri { get; set; }

        [DataMember]
        public TextFormatters text_formatters { get; set; }
    }

    public enum TextFormatters
    {
        Bold = 0x0001,
        Italic = 0x0010,
        Undeline = 0x0100
    }
}
