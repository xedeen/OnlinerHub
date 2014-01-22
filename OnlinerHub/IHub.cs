using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Onliner.Model;

namespace OnlinerHub
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHub" in both code and config file together.
    [ServiceContract]
    public interface IHub
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "article?articleID={articleID}&cursor={cursor}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        ArticleDto GetArticle(long articleId, int cursor);

        [OperationContract]
        [WebInvoke(UriTemplate = "comments?articleID={articleID}&cursor={cursor}", Method = "GET",
            ResponseFormat = WebMessageFormat.Json)]
        CommentPageDto GetComments(long articleId, int cursor);

        [OperationContract]
        [WebInvoke(UriTemplate = "articles?feedType={feedType}", Method = "GET",
            ResponseFormat = WebMessageFormat.Json)]
        List<FeedItemDto> GetFeed(FeedType feedType);
    }

    [DataContract]
    public class FeedItemDto
    {
        public long article_id { get; set; }
        public string title { get; set; }
        public string uri { get; set; } 
    }

    

    [DataContract]
    public class ArticleDto
    {
        [DataMember]
        public string content { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string uri { get; set; }
        [DataMember]
        public int? previous_page_cursor { get; set; }
        [DataMember]
        public int? next_page_cursor { get; set; }
    }

    [DataContract]
    public class CommentPageDto
    {
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
        [DataMember]
        public AuthorDto author { get; set; }
        [DataMember]
        public string content { get; set; }
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
}
