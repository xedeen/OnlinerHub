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

            if (null == page || null == page.ArticleParagraphs)
                return new ArticlePageDto { Error = "Cannot retrieve comments" };

            if (page.ArticleParagraphs.Count > numberOfObjectsPerPage)
            {
                cursorNext = page.ArticleParagraphs.Count > (cursor + 1) * numberOfObjectsPerPage
                                 ? cursor + 1
                                 : cursor;

                page.ArticleParagraphs =
                    page.ArticleParagraphs.Where(
                        c =>
                        c.inner_id >= cursor * numberOfObjectsPerPage && c.inner_id < (cursor + 1) * numberOfObjectsPerPage)
                            .ToList();
            }

            return new ArticlePageDto
            {
                paragraphs = page.ArticleParagraphs,
                next_page_cursor = cursorNext == cursor ? (int?)null : cursorNext,
                previous_page_cursor = cursor > 0 ? cursor - 1 : (int?)null
            };
        }

        private FullArticlePage ProcessArticlePage(string articleUrl)
        {
            var fullPage = new FullArticlePage();

            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(articleUrl + "/comments"))
                fullPage.Comments = cache.Get(articleUrl + "/comments") as List<CommentDto>;
            if (cache.Contains(articleUrl + "/content"))
                fullPage.ArticleParagraphs = cache.Get(articleUrl + "/content") as List<ArticlePageItemBase>;

            if (null == fullPage.Comments || null == fullPage.ArticleParagraphs)
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

                    if (null == fullPage.ArticleParagraphs)
                    {
                        fullPage.ArticleParagraphs = ProcessContentInner(html);
                        cache.Add(articleUrl + "/content", fullPage.ArticleParagraphs,
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

        private List<ArticlePageItemBase> ProcessContentInner(HAP.HtmlDocument html)
        {
            var node = html.DocumentNode.Descendants("article").FirstOrDefault();
            if (node == null) return null;
            int innerId = 0;
            return
                node.SelectNodes("div[@class='b-posts-1-item__text']/p|div[@class='b-posts-1-item__text']/table")
                    .Select(pn => ParseParagraph(pn, innerId++))
                    .ToList();
        }

        private ArticlePageItemBase ParseParagraph(HAP.HtmlNode paragraphNode, int innerId)
        {
            if (paragraphNode.Name == "table")
                return ProcessTableParagraph(paragraphNode, innerId);
            return new ArticleParagraphDto {Content = ProcessChildContent(paragraphNode),inner_id = innerId};
            
        }

        private List<ArticleParagraphContentDto> ProcessChildContent(HAP.HtmlNode node)
        {
            List<ArticleParagraphContentDto> result = null;
            
            foreach (var childNode in node.ChildNodes)
            {
                var processChildren = false;
                ArticleParagraphContentDto content = null;
                switch (childNode.Name)
                {
                    case "#text":
                        content = new ArticleParagraphContentDto
                        {
                            ContentType = "t",
                            Content = childNode.InnerText
                        };
                        break;
                    case "a":
                        processChildren = childNode.HasChildNodes;
                        content = new ArticleParagraphContentDto
                        {
                            ContentType = "a",
                            //Content = childNode.InnerText,
                            Url = childNode.Attributes["href"].Value
                        };
                        break;
                    case "em":
                        processChildren = childNode.HasChildNodes;
                        content = new ArticleParagraphContentDto
                        {
                            ContentType = "i",
                            //Content = childNode.InnerText
                        };
                        break;
                    case "strong":
                        processChildren = childNode.HasChildNodes;
                        content = new ArticleParagraphContentDto
                        {
                            ContentType = "b",
                            //Content = childNode.InnerText
                        };
                        break;
                    case "img":
                        content = new ArticleParagraphContentDto
                        {
                            ContentType = "img",
                            Content =
                                childNode.HasAttributes && childNode.Attributes.Contains("alt")
                                    ? childNode.Attributes["alt"].Value
                                    : string.Empty,
                            Url = childNode.Attributes["src"].Value
                        };
                        break;
                    case "iframe":
                        content = ProcessIFrame(childNode);
                        break;
                }
                if (null != content)
                {
                    if (processChildren)
                        content.ChildContent = ProcessChildContent(childNode);

                    if (null != content.ChildContent || !string.IsNullOrEmpty(content.ContentType))
                    {
                        if (null == result)
                            result = new List<ArticleParagraphContentDto>();
                        result.Add(content);
                    }
                }
            }
            return result;
        }

        private ArticleTableDto ProcessTableParagraph(HAP.HtmlNode tableNode, int innerId)
        {
            var table = new ArticleTableDto {inner_id = innerId};
            var rows = tableNode.SelectNodes("tbody/tr");
            foreach (var rowNode in rows)
            {
                var row = new ArticleTableRowDto();

                var columns = rowNode.SelectNodes("td");
                foreach (var cellNode in columns)
                {
                    var cellParagraph = cellNode.ChildNodes.FirstOrDefault(c => c.Name == "p");
                    var cellContent = ProcessChildContent(cellParagraph ?? cellNode);
                    if (null != cellContent)
                    {
                        var cell = new ArticleTableCellDto
                        {
                            Content = cellContent
                        };
                        if (null == row.Cells)
                            row.Cells = new List<ArticleTableCellDto>();
                        row.Cells.Add(cell);
                    }
                }
                if (null == table.Rows)
                    table.Rows = new List<ArticleTableRowDto>();
                table.Rows.Add(row);
            }
            return table;
        }

        private ArticleParagraphContentDto ProcessIFrame(HAP.HtmlNode node)
        {
            var content = new ArticleParagraphContentDto
            {
                Content = string.Empty,
                ContentType = "v"
            };
            var url = node.HasAttributes && node.Attributes.Contains("src")
                ? node.Attributes["src"].Value
                : string.Empty;
            if (url.StartsWith("//www"))
                url = url.Replace("//www", "http://www");
            var match = Regex.Match(url, @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);
            if (match.Success )
            {
                string id = match.Groups[1].Value;
                content.Thumbnail = string.Format("http://img.youtube.com/vi/{0}/0.jpg", id);
                content.Url = string.Format("http://www.youtube.com/watch?v={0}", id);
                content.VideoId = id;
            }
            else
            {
                match = Regex.Match(url, @"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)");
                if (match.Success)
                    content.VideoId = match.Groups[1].Value;
                content.Url = url;
            }
            return content;
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
