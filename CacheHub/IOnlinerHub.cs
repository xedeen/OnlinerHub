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
        public int InnerId { get; set; }
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
