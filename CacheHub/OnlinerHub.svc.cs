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
using System.Text.RegularExpressions;
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
            var page = ProcessArticlePage(articleUrl);

            if (null == page || null == page.Comments)
                return new CommentsPageDto {Error = "Cannot retrieve comments"};

            if (page.Comments.Count > numberOfObjectsPerPage)
            {
                cusrorNext = page.Comments.Count > (cursor + 1) * numberOfObjectsPerPage
                                 ? cursor + 1
                                 : cursor;

                page.Comments =
                    page.Comments.Where(
                        c =>
                        c.inner_id >= cursor * numberOfObjectsPerPage && c.inner_id < (cursor + 1) * numberOfObjectsPerPage)
                            .ToList();
            }

            return new CommentsPageDto
                       {
                           comments = page.Comments,
                           next_page_cursor = cusrorNext == cursor ? (int?) null : cusrorNext,
                           previous_page_cursor = cursor > 0 ? cursor - 1 : (int?) null
                       };
        }

        public ArticlePageDto GetContent(string articleUrl, int cursor)
        {
            const int numberOfObjectsPerPage = 3;
            var cursorNext = cursor;
            var page = ProcessArticlePage(articleUrl);

            if (null == page || null == page.Article || null == page.Article.Content)
                return new ArticlePageDto {Error = "Cannot retrieve content"};

            var content = page.Article.Content;
            if (page.Article.Content.Count > numberOfObjectsPerPage)
            {
                cursorNext = page.Article.Content.Count > (cursor + 1) * numberOfObjectsPerPage
                                 ? cursor + 1
                                 : cursor;

                content =
                    page.Article.Content.Where(
                        c =>null!=c &&
                        c.InnerId >= cursor * numberOfObjectsPerPage && c.InnerId < (cursor + 1) * numberOfObjectsPerPage)
                            .ToList();
            }

            return new ArticlePageDto
            {
                paragraphs = content,
                next_page_cursor = cursorNext == cursor ? (int?) null : cursorNext,
                previous_page_cursor = cursor > 0 ? cursor - 1 : (int?) null
            };
        }

        public Header GetHeader(string articleUrl)
        {
            var page = ProcessArticlePage(articleUrl);

            if (null == page || null == page.Article || null == page.Article.Header)
                return new Header { Error = "Cannot retrieve header" };
            return page.Article.Header;
        }

        private FullArticlePage ProcessArticlePage(string articleUrl)
        {
            var fullPage = new FullArticlePage();

            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(articleUrl + "/comments"))
                fullPage.Comments = cache.Get(articleUrl + "/comments") as List<CommentDto>;
            if (cache.Contains(articleUrl + "/content"))
                fullPage.Article = cache.Get(articleUrl + "/content") as Article;

            if (null == fullPage.Comments || null == fullPage.Article)
            {
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

                    if (null == fullPage.Article)
                    {
                        var p = new OnlinerParser();
                        fullPage.Article = new Article
                        {
                            Header = p.ParseHeader(html),
                            Content = p.Parse(html)
                        };
                        cache.Add(articleUrl + "/content", fullPage.Article,
                        new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddHours(1.0) });
                    }
                    if (null == fullPage.Comments)
                    {
                        fullPage.Comments = ProcessCommentsInner2(html);
                        cache.Add(articleUrl + "/comments", fullPage.Comments,
                            new CacheItemPolicy {AbsoluteExpiration = DateTime.Now.AddMinutes(5.0)});
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return fullPage;
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
    }
}
