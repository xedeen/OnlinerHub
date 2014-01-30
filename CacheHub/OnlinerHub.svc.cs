using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using HAP = HtmlAgilityPack;

namespace CacheHub
{
    public class OnlinerHub : IOnlinerHub
    {
        public string GetReadability(string articleUrl)
        {
            return string.Format(
                "https://www.readability.com/api/content/v1/parser?url={0}&token=ecd1f4e3683b5bfbd41c02c20229011983d03671",
                articleUrl);
        }

        public CommentsPageDto GetComments(string articleUrl, int cursor)
        {
            const int numberOfObjectsPerPage = 20;
            var cusrorNext = cursor;

            var comments = ProcessComments(articleUrl);

            if (null == comments)
                return new CommentsPageDto {Error = "Cannot retrieve comments"};

            if (comments.Count > numberOfObjectsPerPage)
            {
                cusrorNext = comments.Count > (cursor + 1)*numberOfObjectsPerPage
                                 ? cursor + 1
                                 : cursor;

                comments =
                    comments.Where(
                        c =>
                        c.inner_id >= cursor * numberOfObjectsPerPage && c.inner_id < (cursor + 1) * numberOfObjectsPerPage)
                            .ToList();
            }

            return new CommentsPageDto
                       {
                           comments = comments,
                           next_page_cursor = cusrorNext == cursor ? (int?) null : cusrorNext,
                           previous_page_cursor = cursor > 0 ? cursor - 1 : (int?) null
                       };
        }

        private List<CommentDto> ProcessComments(string articleUrl)
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(articleUrl))
                return cache.Get(articleUrl) as List<CommentDto>;

            try
            {
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(articleUrl);
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                var response = httpWebRequest.GetResponse();

                var html = new HAP.HtmlDocument();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    html.LoadHtml(sr.ReadToEnd());
                }
                var comments = ProcessCommentsInner(html);

                var cacheItemPolicy = new CacheItemPolicy();
                cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddMinutes(5.0);
                cache.Add(articleUrl, comments, cacheItemPolicy);

                return comments;
            }
            catch (Exception e)
            {   
                return null;
            }
        }

        private CiteDto GetBlockQuotes(HAP.HtmlNode commentContent)
        {
            var bqNode = commentContent.ChildNodes.FirstOrDefault(d => d.Name == "blockquote");
            if (null == bqNode) return null;

            var content =
                bqNode.ChildNodes.FirstOrDefault(
                    d =>
                        d.Name == "div");

            var text = "";
            foreach (var node in content.ChildNodes.Where(c => c.Name != "blockquote" && c.Name != "cite"))
            {
                text += node.InnerHtml;
            }
            var cite = content.ChildNodes.FirstOrDefault(c => c.Name == "cite");
            var result = new CiteDto
            {
                child = GetBlockQuotes(content),
                title = null == cite ? string.Empty : cite.InnerText,
                content = GetContent(content)
            };
            return result;
        }

        private ContentDto GetContent(HAP.HtmlNode commentContent)
        {
            var content = new ContentDto();

            foreach (var node in commentContent.ChildNodes.Where(p=>p.Name=="p"))
            {
                content.paragraph_list.Add(ProcessParagraph(node));
            }
            return content;
        }

        private ParagraphDto ProcessParagraph(HAP.HtmlNode node)
        {
            var paragraph = new ParagraphDto();
            if (!node.HasChildNodes)
            {
                paragraph.items.Add(new TextItemDto {content = node.InnerText.Trim('\n')});
                return paragraph;
            }
            foreach (var childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "br":
                        paragraph.items.Add(new TextItemDto {});
                        break;
                    case "strong":
                        paragraph.items.Add(new TextItemDto
                        {
                            content = childNode.InnerText,
                            text_formatters = TextFormatters.Bold
                        });
                        break;
                    case "em":
                        paragraph.items.Add(new TextItemDto
                        {
                            content = childNode.InnerText,
                            text_formatters = TextFormatters.Italic
                        });
                        break;
                    default:
                        paragraph.items.Add(new TextItemDto
                        {
                            content = childNode.InnerText,
                        });
                        break;
                }
            }
            return paragraph;
        }

        private List<CommentDto> ProcessCommentsInner(HAP.HtmlDocument html)
        {
            var node = html.GetElementbyId("onliner_comments").ChildNodes.FirstOrDefault(x => x.Name == "ul");
            if (node == null) return null;

            var commentNodes = node.ChildNodes.Where(c => c.Name == "li");

            var comments = new List<CommentDto>();
            var currentInnerId = 0;

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

                if (commentInfo != null)
                {   
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

                var author = new AuthorDto
                                       {
                                           avatar_source_uri = avatarUrl,
                                           name = commentProfileName,
                                           profile_uri = authorProfile
                                       };

                if (null != commentContent)
                {
                    comments.Add(new CommentDto()
                    {
                        content = GetContent(commentContent),
                        author = author,
                        inner_id = currentInnerId,
                        blockquote = GetBlockQuotes(commentContent)
                    });
                    currentInnerId++;
                }
            }
            return comments;
        }
    }
}
