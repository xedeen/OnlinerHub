using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Onliner.Model;


namespace OnlinerHubService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IHubService
    {
        [OperationContract]
        List<ArticleTitleDto> GetArticles(FeedType feedType);

        [OperationContract]
        ArticleDto GetArticle(long articleId);

        [OperationContract]
        List<CommentDto> GetArticleComments(long articleId);
    }


    [DataContract]
    public class ArticleTitleDto
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Uri { get; set; }

        [DataMember]
        public long ArticleId { get; set; }
    }

    [DataContract]
    public class ArticleDto
    {
        [DataMember]
        public long ArticleId { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Uri { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public List<CommentDto> Comments { get; set; }
    }

    [DataContract]
    public class CommentDto
    {
        [DataMember]
        public AuthorDto Author { get; set; }
        [DataMember]
        public string CommentContent { get; set; }
    }

    [DataContract]
    public class AuthorDto
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ProfileUri { get; set; }
        [DataMember]
        public string AvatarSourceUri { get; set; }
    }
}
