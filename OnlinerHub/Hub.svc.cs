using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Caching;
using Onliner.Common;
using Onliner.Model;
using Onliner.Model.AppModel;

namespace OnlinerHub
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Hub" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Hub.svc or Hub.svc.cs at the Solution Explorer and start debugging.
    public class Hub : IHub
    {
        public List<FeedItemDto> GetFeed(FeedType feedType)
        {
            var items = FeedController.Instance.GetRss(feedType);
            return items.Select(item => new FeedItemDto
                                                  {
                                                      article_id = item.Id,
                                                      title = item.Title,
                                                      uri = item.Uri
                                                  }).ToList();
        }

        public ArticleDto GetArticle(long articleId, int cursor)
        {
            const int maxPageSize = 10000;

            var article = ArticleController.Instance.GetArticle(articleId);
            if (article == null)
                return
                    new ArticleDto {content = "Article is not found"};


            var content = ContainerController.Instance.RetrieveContent(article);
            var cursor2 = cursor;

            var resultContent = content == null ? string.Empty : content.Representation;
            if (resultContent.Length > maxPageSize)
            {
                resultContent = resultContent.Substring(cursor * maxPageSize, maxPageSize);
                if (resultContent.Length > cursor * maxPageSize)
                    cursor2 = cursor + 1;
            }

            return new ArticleDto
                       {
                           content = resultContent,
                           next_page_cursor = cursor2 == cursor ? (int?) null : cursor2,
                           previous_page_cursor = cursor > 0 ? cursor - 1 : (int?) null,
                           title = article.Title,
                           uri = article.Uri
                       };
        }

        public CommentPageDto GetComments(long articleId, int cursor)
        {
            const int numberOfObjectsPerPage = 20;
            var cursor2 = cursor;

            var result = new CommentPageDto();

            var comments = CommentsController.Instance.RetrieveComments(articleId);
            var commentCount = CommentsController.Instance.GetCommentsCount(articleId);

            var authors = AuthorController.Instance.GetAllAuthors();

            if (commentCount > numberOfObjectsPerPage)
            {
                if (commentCount > numberOfObjectsPerPage*cursor)
                    cursor2 = cursor + 1;
            }

            result.comments = (from comment in comments
                               join author in authors on comment.AuthorId equals author.Id
                               select new CommentDto
                                          {
                                              author = new AuthorDto
                                                           {
                                                               avatar_source_uri = author.AvatarUri,
                                                               name = author.Name,
                                                               profile_uri = author.ProfileUri
                                                           },
                                              content = comment.CommentContent
                                          }).Skip(numberOfObjectsPerPage*cursor)
                                            .Take(numberOfObjectsPerPage).ToList();
            result.next_page_cursor = cursor2 == cursor ? (int?) null : cursor2;
            result.previous_page_cursor = cursor > 0 ? cursor - 1 : (int?) null;
            return result;
        }
    }
}
