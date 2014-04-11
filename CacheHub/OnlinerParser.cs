using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HAP = HtmlAgilityPack;

namespace CacheHub
{
    public class OnlinerParser : IParser
    {
        public List<ParagraphBase> Parse(HAP.HtmlDocument html)
        {
            var node = html.DocumentNode.Descendants("article").FirstOrDefault();
            if (node == null) return null;
            int innerId = 0;
            return
                node.SelectNodes("div[@class='b-posts-1-item__text']/p|div[@class='b-posts-1-item__text']/table")
                    .Select(pn => ParseParagraph(pn, innerId++))
                    .ToList();
        }

        public Header ParseHeader(HAP.HtmlDocument html)
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
                case"p":
                    return ParseParagraphInner(paragraphNode, innerId);
                case "table":
                    return ParseTableInner(paragraphNode, innerId);
            }
            return null;
        }

        private Table ParseTableInner(HAP.HtmlNode tableNode, int innerId)
        {
            var table = new Table {InnerId = innerId};
            var rows = tableNode.SelectNodes("tbody/tr");
            foreach (var rowNode in rows)
            {
                var row = new TableRow {Content = new List<P>()};
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

        private List<BaseContentItem> ParseNode(HAP.HtmlNode node, TextModifiers modifiers)
        {
            var result = new List<BaseContentItem>();
            foreach (var childNode in node.ChildNodes)
            {   
                switch (childNode.Name)
                {
                    case "#text":
                        result.Add(
                            new TextBlock
                            {
                                Text = childNode.InnerText,
                                TextModifiers = (int) modifiers
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