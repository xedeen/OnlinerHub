using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Onliner.Common;
using Onliner.Model;


namespace OnlinerHubService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)] 
    public class OnlinerHubService : IHubService
    {
        public List<ArticleTitleDto> GetArticles(FeedType feedType)
        {
            return FeedController.Instance.GetRss(feedType).Select(
                item => new ArticleTitleDto
                            {
                                Title = item.Title,
                                Uri = item.Uri,
                                ArticleId = item.Id
                            }).ToList();
        }

        public ArticleDto GetArticle(long articleId)
        {
            var article = ArticleController.Instance.GetArticle(articleId);
            if (article == null) return null;
            var content = ContainerController.Instance.RetrieveContent(article);

            var dto = new ArticleDto
                          {
                              ArticleId = article.Id,
                              Title = article.Title,
                              Uri = article.Uri,
                              Text = null == content ? string.Empty : content.Representation,
                              //Comments = commentData.ToList(),
                          };
            return dto;
        }

        public List<CommentDto> GetArticleComments(long articleId)
        {
            var comments = CommentsController.Instance.RetrieveComments(articleId);
            var authors = AuthorController.Instance.GetAllAuthors();

            return (from comment in comments
                               join author in authors on comment.AuthorId equals author.Id
                               select new CommentDto
                               {
                                   Author = new AuthorDto
                                   {
                                       AvatarSourceUri = author.AvatarUri,
                                       Name = author.Name,
                                       ProfileUri = author.ProfileUri
                                   },
                                   CommentContent = comment.CommentContent
                               }).ToList();
        }
    }
}
