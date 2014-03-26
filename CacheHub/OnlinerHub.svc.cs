using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
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
                var comments = ProcessCommentsInner2(html);

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

        private List<CommentDto> ProcessCommentsInner2(HAP.HtmlDocument html)
        {
            var node = html.GetElementbyId("onliner_comments").ChildNodes.FirstOrDefault(x => x.Name == "ul");
            if (node == null) return null;

            var innerId = 0;
            return node.SelectNodes("li[@data-comment-id]").Select(commentNode => ParseComment(commentNode, innerId++)).ToList();
        }

        private CommentDto ParseComment(HAP.HtmlNode commentNode, int innerId)
        {
            
            var id = 0;
            Int32.TryParse(commentNode.Attributes["data-comment-id"].Value, out id);
            return new CommentDto
            {
                inner_id = innerId,
                author = ParseAuthor(commentNode.SelectSingleNode("div/strong[@class='author']")),
                content = ParseContent(commentNode.SelectSingleNode("div[@class='comment-content']"))
            };
        }
        
        private AuthorDto ParseAuthor(HAP.HtmlNode node)
        {
            if (null == node) return null;
            var link = node.SelectSingleNode("a[@href]");
            var img = node.SelectSingleNode("a/figure[@class='author-image']/img[@src]");
            return new AuthorDto
            {
                profile_uri = link == null ? string.Empty : link.Attributes["href"].Value,
                avatar_source_uri = img == null ? string.Empty : img.Attributes["src"].Value,
                name = null == link ? string.Empty : link.InnerText
            };
        }

        private ContentDto ParseContent(HAP.HtmlNode content)
        {
            var currentContent = new ContentDto();
            var currentFormatting = string.Empty;
            ProcessNode(content, null, currentContent, currentFormatting);
            return currentContent;
        }

        private void ProcessNode(HAP.HtmlNode node, BlockItemDto currentBlock, ContentDto commentContent, string currentFormatting)
        {
            foreach (var child in node.ChildNodes)
            {
                if (currentBlock == null)
                {
                    currentBlock=new BlockItemDto();
                    currentBlock.content = new ParagraphDto();
                    commentContent.items.Add(currentBlock);
                }

                switch (child.Name)
                {
                    case "blockquote":
                        var newBlock = new BlockItemDto {is_blockquote = true};
                        ProcessNode(child.SelectSingleNode("div"), newBlock, commentContent, currentFormatting);
                        if (currentBlock.is_blockquote)
                        {
                            if (null == currentBlock.children)
                                currentBlock.children = new List<BlockItemDto>();
                            currentBlock.children.Add(newBlock);
                        }
                        else
                        {
                            commentContent.items.Add(newBlock);
                            currentBlock = null;
                        }
                        break;
                    case "cite":
                        currentBlock.title = child.InnerText.Trim('\n');
                        break;
                    case "p":
                        ProcessNode(child, currentBlock, commentContent, currentFormatting);
                        //ProcessNode(child, currentBlock, commentContent,
                          //  ProcessBlockItem(child, currentBlock, currentFormatting));
                        break;
                    case "strong":
                        if (!currentFormatting.Contains("B"))
                            currentFormatting += "B";
                        ProcessNode(child, currentBlock, commentContent, currentFormatting);
                        break;
                    case "em":
                        if (!currentFormatting.Contains("I"))
                            currentFormatting += "I";
                        ProcessNode(child, currentBlock, commentContent, currentFormatting);
                        break;
                    case "#text":
                        ProcessBlockItem(child, currentBlock, currentFormatting);
                        break;
                    case "a":
                        var text = child.InnerText.Trim('\n').Trim();
                        if (!string.IsNullOrEmpty(text))
                            currentBlock.content.items.Add(new ParagraphItem
                            {
                                content = text,
                                type = currentFormatting,
                                link = child.Attributes["href"].Value
                            });
                        break;
                }
            }
        }

        private string ProcessBlockItem(HAP.HtmlNode node, BlockItemDto currentBlock, string currentFormatting, string format = null)
        {
            var text = node.InnerText.Trim('\n').Trim();
            if (!string.IsNullOrEmpty(format) && !currentFormatting.Contains(format))
                currentFormatting += format;

            if (!string.IsNullOrEmpty(text))
            {
                currentBlock.content.items.Add(new ParagraphItem
                {
                    content = text,
                    type = currentFormatting
                });
            }
            return currentFormatting;
        }


        //private ParagraphDto ProcessBlockQuote(HAP.HtmlNode bqNode)
        //{
        //    var content =
        //        bqNode.ChildNodes.FirstOrDefault(
        //            d =>
        //                d.Name == "div");
        //    if (null == content) return null;

        //    var title = content.ChildNodes.FirstOrDefault(c => c.Name == "cite");
        //    ParagraphDto paragraph = null;
        //    var children = new List<ParagraphDto>();

        //    foreach (var node in content.ChildNodes)
        //    {
        //        switch (node.Name)
        //        {
        //            case "p":
        //            case "#text":
        //                paragraph = ProcessParagraph(node);
        //                break;
        //            case "blockquote":
        //                children.Add(ProcessBlockQuote(node));
        //                break;
        //        }
        //    }
        //    if (null == paragraph) paragraph = new ParagraphDto();
        //    paragraph.IsBlockQuote = true;
        //    paragraph.CiteTitle = title == null ? string.Empty : title.InnerText;
        //    paragraph.children = children;

        //    return paragraph;
        //}

        //private ContentDto GetContent(HAP.HtmlNode commentContent)
        //{
        //    var content = new ContentDto();

        //    foreach (var node in commentContent.ChildNodes)
        //    {
        //        switch (node.Name)
        //        {
        //            case "p":
        //            case "#text":
        //                content.paragraph_list.Add(ProcessParagraph(node));
        //                break;
        //            case "blockquote":
        //                content.paragraph_list.Add(ProcessBlockQuote(node));
        //                break;
        //        }
        //    }
        //    return content;
        //}


        //private ParagraphDto ProcessParagraph(HAP.HtmlNode node)
        //{
        //    ParagraphDto paragraph;
        //    switch (node.Name)
        //    {
        //        case "p":
        //        case "#text":
        //            paragraph = new ParagraphDto();
        //            if (!node.HasChildNodes)
        //            {
        //                var text = node.InnerText.Trim('\n').Trim();
        //                if (!string.IsNullOrEmpty(text))
        //                    paragraph.items.Add(new TextItemDto {content = text});
        //                else
        //                    paragraph = null;
        //                return paragraph;
        //            }
        //            foreach (var childNode in node.ChildNodes)
        //            {
        //                switch (childNode.Name)
        //                {
        //                    case "br":
        //                        paragraph.items.Add(new TextItemDto());
        //                        break;
        //                    case "strong":
        //                        paragraph.items.Add(new TextItemDto
        //                        {
        //                            content = childNode.InnerText.Trim('\n'),
        //                            text_formatters = (int) TextFormatters.Bold
        //                        });
        //                        break;
        //                    case "em":
        //                        paragraph.items.Add(new TextItemDto
        //                        {
        //                            content = childNode.InnerText.Trim('\n'),
        //                            text_formatters = (int) TextFormatters.Italic
        //                        });
        //                        break;
        //                    case "cite":
        //                        break;
        //                    case "a":
        //                        var lnk = childNode.HasAttributes && childNode.Attributes.Contains("href")
        //                            ? childNode.Attributes["href"].Value
        //                            : string.Empty;
        //                        paragraph.items.Add(new TextItemDto
        //                        {
        //                            content = childNode.InnerText.Trim('\n'),
        //                            text_formatters = (int) TextFormatters.Undeline,
        //                            link_uri = Uri.IsWellFormedUriString(lnk, UriKind.Absolute)
        //                                ? new Uri(lnk)
        //                                : null
        //                        });
        //                        break;
        //                    default:
        //                        paragraph.items.Add(new TextItemDto
        //                        {
        //                            content = childNode.InnerText.Trim('\n'),
        //                        });
        //                        break;
        //                }
        //            }
        //            break;
        //        case "blockquote":
        //            paragraph = ProcessBlockQuote(node);
        //            break;
        //        default:
        //            paragraph = null;
        //            break;
        //    }
        //    return paragraph;
        //}

        //private List<CommentDto> ProcessCommentsInner(HAP.HtmlDocument html)
        //{
        //    var node = html.GetElementbyId("onliner_comments").ChildNodes.FirstOrDefault(x => x.Name == "ul");
        //    if (node == null) return null;

        //    var commentNodes = node.ChildNodes.Where(c => c.Name == "li");

        //    var comments = new List<CommentDto>();
        //    var currentInnerId = 0;

        //    foreach (var commentNode in commentNodes)
        //    {
        //        long commentId = 0;

        //        if (commentNode.Attributes.Any(a => a.Name == "data-comment-id"))
        //            Int64.TryParse(commentNode.Attributes["data-comment-id"].Value, out commentId);

        //        var commentInfo =
        //            commentNode.ChildNodes.FirstOrDefault(
        //                d =>
        //                d.Name == "div" && d.HasAttributes &&
        //                d.Attributes.Any(a => a.Name == "class" && a.Value == "comment-info"));

        //        var commentContent =
        //            commentNode.ChildNodes.FirstOrDefault(
        //                d =>
        //                d.Name == "div" && d.HasAttributes &&
        //                d.Attributes.Any(a => a.Name == "class" && a.Value == "comment-content"));

        //        #region LIKES

        //        //var commentActions =
        //        //    commentNode.ChildNodes.FirstOrDefault(
        //        //        d => d.Name == "div" && d.HasAttributes && d.Attributes.Any(a => a.Name == "class" && a.Value == "comment-actions"));

        //        //if (null != commentActions && commentActions.ChildNodes.Any(c=>c.Name=="a"))
        //        //{
        //        //    var span =
        //        //        commentActions.ChildNodes.First(c => c.Name == "a")
        //        //                      .ChildNodes.FirstOrDefault(cc => cc.Name == "span");

        //        //    if (null != span)
        //        //        int.TryParse(span.InnerText, out likesCount);
        //        //}

        //        #endregion

        //        var authorProfile = string.Empty;
        //        var avatarUrl = string.Empty;
        //        var commentProfileName = string.Empty;

        //        if (commentInfo != null)
        //        {   
        //            var strong = commentInfo.ChildNodes.FirstOrDefault(n => n.Name == "strong");
        //            if (strong != null)
        //            {
        //                var infoNode = strong.ChildNodes.FirstOrDefault(n => n.Name == "a");
        //                if (infoNode != null)
        //                {
        //                    authorProfile = (infoNode.Attributes.Any(a => a.Name == "href"))
        //                                        ? infoNode.Attributes["href"].Value
        //                                        : string.Empty;
        //                    if (infoNode.ChildNodes.Count > 0 && infoNode.ChildNodes.First().ChildNodes.Count > 0)
        //                        avatarUrl = infoNode.ChildNodes[0].ChildNodes[0].Attributes.Any(a => a.Name == "src")
        //                                        ? infoNode.ChildNodes[0].ChildNodes[0].Attributes["src"].Value
        //                                        : string.Empty;
        //                    commentProfileName = infoNode.InnerText.Trim('\"');
        //                }
        //            }
        //        }

        //        var author = new AuthorDto
        //                               {
        //                                   avatar_source_uri = avatarUrl,
        //                                   name = commentProfileName,
        //                                   profile_uri = authorProfile
        //                               };

        //        if (null != commentContent)
        //        {
        //            comments.Add(new CommentDto()
        //            {
        //                content = GetContent(commentContent),
        //                author = author,
        //                inner_id = currentInnerId
        //            });
        //            currentInnerId++;
        //        }
        //    }
        //    return comments;
        //}
    }
}
