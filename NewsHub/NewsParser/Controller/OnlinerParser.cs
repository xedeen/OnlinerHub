using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NewsParser.Controller.Interface;
using NewsParser.Model;
using NewsParser.Model.Base;
using NewsParser.Model.Events;
using HAP=HtmlAgilityPack;

namespace NewsParser.Controller
{
    public class OnlinerParser : IParser
    {
        public event ArticleCompleteDelegate ArticleParsed;
        public event CommentsCompleteDelegate CommentsParsed;

        private HAP.HtmlDocument _html = null;

        public OnlinerParser(HAP.HtmlDocument doc)
        {
            _html = doc;
        }

        public void ParseArticleAsync()
        {
            Task.Factory.StartNew(ParseArticleAsyncTask);
        }

        public void ParseCommentsAsync()
        {
             Task.Factory.StartNew(ParseCommentsAsyncTask);
        }

        private async void ParseArticleAsyncTask()
        {
            var success = true;
            List<ParagraphBase> result = null;
            Header header = null;
            long postId = 0;

            try
            {
                result = Parse(_html, out postId);
                header = ParseHeader(_html);
                _html = new HAP.HtmlDocument();
                _html.LoadHtml(_html.DocumentNode.InnerHtml);
            }
            catch (Exception e)
            {
                success = false;
            }

            if (null != ArticleParsed)
                ArticleParsed.Invoke(this, new ArticleCompleteEventArgs
                {
                    Article = new Article {Content = result, Header = header, PostId = postId},
                    Success = success
                });
        }

        private async void ParseCommentsAsyncTask()
        {
            if (null == _html) return;
            var result = new List<Comment>();
            var success = false;

            var node = _html.GetElementbyId("onliner_comments").ChildNodes.FirstOrDefault(x => x.Name == "ul");
            if (node != null)
            {
                var innerId = 0;
                result =
                    node.SelectNodes("li[@data-comment-id]")
                        .Select(commentNode => ParseComment(commentNode, innerId++))
                        .Where(comment => null != comment)
                        .ToList();
                success = true;
            }

            if (null != CommentsParsed)
                CommentsParsed.Invoke(this, new CommentsCompleteEventArgs
                {
                    Comments = result,
                    Success = success
                });
        }




        private void OnLoadComplete(object sender, LoadCompleteEventArgs args)
        {
            if (!args.Success || null == args.Page)
            {
                if (ArticleParsed != null)
                    ArticleParsed(this, new ArticleCompleteEventArgs {Success = false});
                _html = null;
                return;
            }


            var success = true;
            List<ParagraphBase> result = null;
            Header header = null;
            long postId = 0;
            
            try
            {
                result = Parse(args.Page, out postId);
                header = ParseHeader(args.Page);
                _html = new HAP.HtmlDocument();
                _html.LoadHtml(args.Page.DocumentNode.InnerHtml);
            }
            catch (Exception e)
            {
                success = false;
            }

            Task.Factory.StartNew(ParseCommentsAsync);

            if (null != ArticleParsed)
                ArticleParsed.Invoke(this, new ArticleCompleteEventArgs
                {
                    Article = new Article {Content = result, Header = header, PostId = postId},
                    Success = success
                });
        }


        public List<ParagraphBase> Parse(HAP.HtmlDocument html, out long postId)
        {
            postId = ParsePostId(html);
            var node = html.DocumentNode.Descendants("article").FirstOrDefault();
            if (node == null) return null;
            int innerId = 0;
            return
                node.SelectNodes("div[@class='b-posts-1-item__text']/p|div[@class='b-posts-1-item__text']/table")
                    .Select(pn => ParseParagraph(pn, innerId++))
                    .ToList();
        }

        private Header ParseHeader(HAP.HtmlDocument html)
        {
            var node = html.DocumentNode.Descendants("article").FirstOrDefault();
            if (node == null) return null;
            return new Header
            {
                Title = node.SelectSingleNode("h3[@class='b-posts-1-item__title']/a/span").InnerText,
                Tags =
                    node.SelectNodes("div[@class='b-post-tags-1']/strong/a|div[@class='b-post-tags-1']/small/a")
                        .Select(ParseTag)
                        .ToList(),
                Image = ParseImage(node.SelectSingleNode("figure[@class='b-posts-1-item__image']/img"))
            };
        }

        private long ParsePostId(HAP.HtmlDocument html)
        {
            var node =
                html.DocumentNode.Descendants("form")
                    .FirstOrDefault(
                        n =>
                            n.HasAttributes && null != n.Attributes["class"] &&
                            n.Attributes["class"].Value == "b-form-postmsg");
            if (node == null) return 0;
            
            var hiddenInput = node.SelectSingleNode(
                "div[@class='b-comments-form']/div[@class='b-comments-form-button']/input[@name='postId']");
            if (null == hiddenInput || !hiddenInput.HasAttributes || null == hiddenInput.Attributes["value"])
                return 0;
            long retVal = 0;
            Int64.TryParse(hiddenInput.Attributes["value"].Value, out retVal);
            return retVal;
        }

        private Image ParseImage(HAP.HtmlNode node)
        {
            if (null == node) return null;
            return new Image
            {
                SourceUrl = node.Attributes["src"].Value
            };
        }


        private Tag ParseTag(HAP.HtmlNode node)
        {
            return new Tag
            {
                Text = node.InnerText,
                HRef = node.Attributes["href"].Value,
                Size = node.ParentNode.Name == "strong" ? TagSize.Big : TagSize.Small
            };
        }
        

        private Comment ParseComment(HAP.HtmlNode commentNode, int innerId)
        {
            long id = 0;
            Int64.TryParse(commentNode.Attributes["data-comment-id"].Value, out id);

            try
            {
                var comment = new Comment();
                comment.Author = ParseAuthor(commentNode.SelectSingleNode("div/strong[@class='author']"));
                comment.Content = ParseContent(commentNode.SelectSingleNode("div[@class='comment-content']"));
                comment.Id = id;
                comment.InnerId = innerId;

                return comment;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private CommentAuthor ParseAuthor(HAP.HtmlNode node)
        {
            if (null == node) return null;
            var link = node.SelectSingleNode("a[@href]");
            var img = node.SelectSingleNode("a/figure[@class='author-image']/img[@src]");
            return new CommentAuthor
            {
                ProfileUrl = link == null ? string.Empty : link.Attributes["href"].Value,
                AvatarSourceUrl = img == null ? string.Empty : img.Attributes["src"].Value,
                Name = null == link ? string.Empty : link.InnerText
            };
        }

        private List<BlockItem> ParseContent(HAP.HtmlNode content)
        {
            var currentContent = new List<BlockItem>();
            ProcessNode(content, null, currentContent, 0);
            return currentContent;
        }

        private void ProcessNode(HAP.HtmlNode node, BlockItem currentBlock, List<BlockItem> blocks, TextModifiers modifiers)
        {
            foreach (var child in node.ChildNodes)
            {
                if (currentBlock == null)
                {
                    currentBlock = new BlockItem {Content = new P()};
                    blocks.Add(currentBlock);
                }

                switch (child.Name)
                {
                    case "blockquote":
                        var newBlock = new BlockItem {IsBlockquote = true};
                        ProcessNode(child.SelectSingleNode("div"), newBlock, blocks, 0);
                        if (currentBlock.IsBlockquote)
                        {
                            if (null == currentBlock.Children)
                                currentBlock.Children = new List<BlockItem>();
                            currentBlock.Children.Add(newBlock);
                        }
                        else
                        {
                            blocks.Add(newBlock);
                            currentBlock = null;
                        }
                        break;
                    case "cite":
                        currentBlock.Title = child.InnerText.Trim('\n');
                        break;
                    case "p":
                        ProcessNode(child, currentBlock, blocks, 0);
                        //ProcessNode(child, currentBlock, commentContent,
                        //  ProcessBlockItem(child, currentBlock, currentFormatting));
                        break;
                    case "strong":
                        ProcessNode(child, currentBlock, blocks, modifiers | TextModifiers.Bold);
                        break;
                    case "em":
                        ProcessNode(child, currentBlock, blocks, modifiers | TextModifiers.Italic);
                        break;
                    case "#text":
                        ProcessBlockItem(child, currentBlock, modifiers);
                        break;
                    case "a":
                        var text = child.InnerText.Trim('\n').Trim();
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (null == currentBlock.Content)
                                currentBlock.Content = new P();
                            if (null == currentBlock.Content.Items)
                                currentBlock.Content.Items = new List<ContentItemBase>();

                            currentBlock.Content.Items.Add(new A
                            {
                                Text = text,
                                HRef = child.Attributes["href"].Value
                            });
                        }
                        break;
                }
            }
        }

        private void ProcessBlockItem(HAP.HtmlNode node, BlockItem currentBlock, TextModifiers modifiers)
        {
            var text = node.InnerText.Trim('\n').Trim();
            
            if (!string.IsNullOrEmpty(text))
            {
                if (null == currentBlock.Content)
                    currentBlock.Content = new P();
                if (null == currentBlock.Content.Items)
                    currentBlock.Content.Items = new List<ContentItemBase>();

                currentBlock.Content.Items.Add(new TextBlock
                {
                    Text = text,
                    TextModifiers = (int) modifiers
                });
            }
        }

        public void CreateXaml(List<ParagraphBase> paragraphs)
        {
            var xamlBuilder = new StringBuilder();
            foreach (var paragraph in paragraphs)
            {
                CreateParagraph(paragraph, xamlBuilder);
            }
            var s = xamlBuilder.ToString();
        }

        private void CreateParagraph(ParagraphBase paragraph, StringBuilder xamlBuilder)
        {

        }

        private ParagraphBase ParseParagraph(HAP.HtmlNode paragraphNode, int innerId)
        {
            switch (paragraphNode.Name)
            {
                case "p":
                    return ParseParagraphInner(paragraphNode, innerId);
                case "table":
                    return ParseTableInner(paragraphNode, innerId);
            }
            return null;
        }

        private Table ParseTableInner(HAP.HtmlNode tableNode, int innerId)
        {
            var table = new Table() { InnerId = innerId };
            var rows = tableNode.SelectNodes("tbody/tr");
            foreach (var rowNode in rows)
            {
                var row = new TableRow { Content = new List<P>() };
                var columns = rowNode.SelectNodes("td");
                foreach (var cellNode in columns)
                {
                    var cellParagraph = cellNode.ChildNodes.FirstOrDefault(c => c.Name == "p");
                    row.Content.Add(new P
                    {
                        Items = ParseNode(cellParagraph ?? cellNode, 0)
                    });
                }
                if (null == table.Rows)
                    table.Rows = new List<TableRow>();
                table.Rows.Add(row);
            }
            return table;
        }

        private P ParseParagraphInner(HAP.HtmlNode paragraphNode, int innerId)
        {
            var p = new P()
            {
                InnerId = innerId,
                Items = ParseNode(paragraphNode, 0)
            };
            return p.Items.Count > 0 ? p : null;
        }

        private List<ContentItemBase> ParseNode(HAP.HtmlNode node, TextModifiers modifiers)
        {
            var result = new List<ContentItemBase>();
            foreach (var childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "#text":
                        result.Add(
                            new TextBlock
                            {
                                Text = childNode.InnerText,
                                TextModifiers = (int)modifiers
                            });
                        break;
                    case "a":
                        var innerContentList = ParseNode(childNode, modifiers);
                        var imageContent = innerContentList.FirstOrDefault(item => item is Image);
                        if (null != imageContent)
                        {
                            result.Add(new Image
                            {
                                HRef = childNode.Attributes["href"].Value,
                                SourceUrl = (imageContent as Image).SourceUrl
                            });
                        }
                        else
                        {
                            var textContent = innerContentList.FirstOrDefault(item => item is TextBlock);
                            if (null != textContent)
                            {
                                result.Add(new A
                                {
                                    HRef = childNode.Attributes["href"].Value,
                                    Text = (textContent as TextBlock).Text
                                });
                            }
                        }
                        break;
                    case "em":
                        result.AddRange(ParseNode(childNode, modifiers | TextModifiers.Italic));
                        break;
                    case "strong":
                        result.AddRange(ParseNode(childNode, modifiers | TextModifiers.Bold));
                        break;
                    case "img":
                        result.Add(new Image
                        {
                            SourceUrl = childNode.Attributes["src"].Value
                        });
                        break;
                    case "iframe":
                        result.Add(ProcessVideo(childNode));
                        break;
                }
            }
            return result;
        }

        private Video ProcessVideo(HAP.HtmlNode node)
        {
            var content = new Video();

            var url = node.HasAttributes && node.Attributes.Contains("src")
                ? node.Attributes["src"].Value
                : string.Empty;
            if (url.StartsWith("//www"))
                url = url.Replace("//www", "http://www");
            var match = Regex.Match(url, @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string id = match.Groups[1].Value;
                content.SourceUrl = string.Format("http://img.youtube.com/vi/{0}/0.jpg", id);
                content.HRef = string.Format("http://www.youtube.com/watch?v={0}", id);
                content.VideoId = id;
            }
            else
            {
                match = Regex.Match(url, @"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)");
                if (match.Success)
                    content.VideoId = match.Groups[1].Value;
                content.HRef = url;
            }
            return content;
        }
    }
}
