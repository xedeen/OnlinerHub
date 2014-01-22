using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Onliner.Model.AppModel;
using Onliner.Model.Repository;
using HAP = HtmlAgilityPack;

namespace Onliner.Common
{
    public class CommentsController : ControllerBase<CommentsController>
    {
        public void PushComments(long articleId)
        {
            RetrieveComments(articleId);
        }

        public int GetCommentsCount(long articleId)
        {
            var commentrepository = new CommentRepository();
            return commentrepository.GetAll().Count(c => c.ArticleId.Equals(articleId));
        }

        public List<Comment> RetrieveComments(long articleId)
        {
            var commentrepository = new CommentRepository();
            var articleRepository = new ArticleRepository();
            var article = articleRepository.GetAll().Get(articleId);

            var comments = commentrepository.GetAll().CommentsOf(articleId).ToList();
            if (article.LastUpdate.HasValue && (DateTime.Now - article.LastUpdate.Value).TotalMinutes < 10)
            {
                return comments;
            }

            try
            {
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(article.Uri);
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                var response = httpWebRequest.GetResponse();

                var html = new HAP.HtmlDocument();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    html.LoadHtml(sr.ReadToEnd());
                }
                comments = ProcessCommentsInner(html, article.Id);
            }
            catch
            {
            }

            if (null != comments && comments.Count > 0)
            {
                foreach (var item in commentrepository.GetAll().Where(c => c.ArticleId.Equals(article.Id)))
                {
                    commentrepository.Delete(item);
                }

                foreach (var comment in comments)
                {
                    commentrepository.Save(comment);
                }
            }
            return comments;
        }

        private List<Comment> ProcessCommentsInner(HAP.HtmlDocument html, long articleId)
        {
            var node = html.GetElementbyId("onliner_comments").ChildNodes.FirstOrDefault(x => x.Name == "ul");
            if (node == null) return null;

            var commentNodes = node.ChildNodes.Where(c => c.Name == "li");

            var comments = new List<Comment>();
            foreach (var commentNode in commentNodes)
            {
                long commentId = 0;

                if (commentNode.Attributes.Any(a => a.Name == "data-comment-id"))
                    Int64.TryParse(commentNode.Attributes["data-comment-id"].Value, out commentId);

                var commentInfo =
                    commentNode.ChildNodes.FirstOrDefault(
                        d =>
                        d.Name == "div" && d.HasAttributes &&
                        d.Attributes.Any(a => a.Name == "class" && a.Value == "comment-info"));

                var commentContent =
                    commentNode.ChildNodes.FirstOrDefault(
                        d =>
                        d.Name == "div" && d.HasAttributes &&
                        d.Attributes.Any(a => a.Name == "class" && a.Value == "comment-content"));

                #region LIKES

                //var commentActions =
                //    commentNode.ChildNodes.FirstOrDefault(
                //        d => d.Name == "div" && d.HasAttributes && d.Attributes.Any(a => a.Name == "class" && a.Value == "comment-actions"));

                //if (null != commentActions && commentActions.ChildNodes.Any(c=>c.Name=="a"))
                //{
                //    var span =
                //        commentActions.ChildNodes.First(c => c.Name == "a")
                //                      .ChildNodes.FirstOrDefault(cc => cc.Name == "span");

                //    if (null != span)
                //        int.TryParse(span.InnerText, out likesCount);
                //}

                #endregion

                var authorProfile = string.Empty;
                var avatarUrl = string.Empty;
                var commentProfileName = string.Empty;
                var commentInfoText = string.Empty;

                if (commentInfo != null)
                {
                    commentInfoText = commentInfo.OuterHtml;
                    var strong = commentInfo.ChildNodes.FirstOrDefault(n => n.Name == "strong");
                    if (strong != null)
                    {
                        var infoNode = strong.ChildNodes.FirstOrDefault(n => n.Name == "a");
                        if (infoNode != null)
                        {
                            authorProfile = (infoNode.Attributes.Any(a => a.Name == "href"))
                                                ? infoNode.Attributes["href"].Value
                                                : string.Empty;
                            if (infoNode.ChildNodes.Count > 0 && infoNode.ChildNodes.First().ChildNodes.Count > 0)
                                avatarUrl = infoNode.ChildNodes[0].ChildNodes[0].Attributes.Any(a => a.Name == "src")
                                                ? infoNode.ChildNodes[0].ChildNodes[0].Attributes["src"].Value
                                                : string.Empty;
                            commentProfileName = infoNode.InnerText.Trim('\"');
                        }
                    }
                }

                Author author = null;
                if (!string.IsNullOrEmpty(authorProfile) && !string.IsNullOrEmpty(commentProfileName))
                {
                    author = AuthorController.Instance.RetrieveAuthor(authorProfile, commentProfileName, avatarUrl);
                }
                if (null != author && null != commentContent)
                {
                    comments.Add(new Comment
                                     {
                                         ArticleId = articleId,
                                         AuthorId = author.Id,
                                         CommentContent = commentContent.OuterHtml
                                     });
                }
            }
            return comments;
        }
    }
}
