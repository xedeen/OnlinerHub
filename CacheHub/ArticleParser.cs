using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Schema;
using CacheHub.Model2;
using CacheHub.Templates;
using HAP=HtmlAgilityPack;

namespace CacheHub
{
    public class ArticleParser
    {
        private readonly TEXTBLOCK _textBlockTemplate = new TEXTBLOCK();
        private readonly Templates.A _linkTemplate = new Templates.A();
        private readonly Templates.MEDIA _mediaTemplate = new Templates.MEDIA();

        private readonly string _pStart = (new P_START()).TransformText();
        private readonly string _pEnd = (new P_END()).TransformText();

        private string _latestLink;

        public List<ContentItem> Parse(string articleUri)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(articleUri);
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            var response = httpWebRequest.GetResponse();

            try
            {
                var html = new HAP.HtmlDocument();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    html.LoadHtml(sr.ReadToEnd());
                }
                return Parse(html);
            }
            catch (Exception e)
            {
                
            }
            return null;
        }

        public List<ContentItem> Parse(HAP.HtmlDocument html)
        {
            var node = html.DocumentNode.Descendants("article").FirstOrDefault();
            if (null==node) return null;
            int innerId = 0;
            return
                node.SelectNodes("div[@class='b-posts-1-item__text']/p|div[@class='b-posts-1-item__text']/table")
                    .Select(pn => ParseParagraph(pn, innerId++))
                    .ToList();
        }

        private ContentItem ParseParagraph(HAP.HtmlNode paragraphNode, int innerId)
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

        private ContentItem ParseTableInner(HAP.HtmlNode tableNode, int innerId)
        {
            return new ContentItem {InnerId = innerId, ContentType = ContentItemType.Content, Xaml = string.Empty};
            /*var table = new Table() { InnerId = innerId };
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
            return table;*/
        }

        private ContentItem ParseParagraphInner(HAP.HtmlNode paragraphNode, int innerId)
        {
            var xaml = ParseNode(paragraphNode, 0);
            if (!string.IsNullOrEmpty(xaml))
            {
                xaml = _pStart + xaml + _pEnd;
                return new ContentItem
                {
                    InnerId = innerId,
                    ContentType = ContentItemType.Content,
                    Xaml = xaml,
                    LatestLink = _latestLink
                };
            }
            _latestLink = string.Empty;
            return null;
        }


        private string ParseNode(HAP.HtmlNode node, TextModifiers modifiers, string parentLinkSource=null)
        {
            //var result = new List<ContentItemBase>();
            var sb = new StringBuilder();

            foreach (var childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "#text":
                        if (string.IsNullOrEmpty(parentLinkSource))
                        {
                            _textBlockTemplate.Session = new Dictionary<string, object>
                            {
                                {"TEXT_CONTENT", childNode.InnerText.Replace("&nbsp", string.Empty)},
                                {"IS_BOLD", ((int) modifiers | 0x0001) == (int) modifiers},
                                {"IS_ITALIC", ((int) modifiers | 0x0010) == (int) modifiers}
                            };
                            _textBlockTemplate.Initialize();
                            sb.Append(_textBlockTemplate.TransformText());
                        }
                        else
                        {
                            _linkTemplate.Session = new Dictionary<string, object>
                            {
                                {"HREF", parentLinkSource},
                                {"TEXT_CONTENT", childNode.InnerText.Replace("&nbsp", string.Empty)}
                            };
                            _linkTemplate.Initialize();
                            sb.Append(_linkTemplate.TransformText());
                        }
                        break;
                    case "a":
                        sb.Append(ParseNode(childNode, modifiers, childNode.Attributes["href"].Value));
                        break;
                    case "em":
                        sb.Append(ParseNode(childNode, modifiers | TextModifiers.Italic));
                        break;
                    case "strong":
                        sb.Append(ParseNode(childNode, modifiers | TextModifiers.Bold));
                        break;
                    case "img":
                        _mediaTemplate.Session=new Dictionary<string, object>
                        {
                            {"HREF", parentLinkSource},
                            {"IMAGE_SOURCE", childNode.Attributes["src"].Value},
                            {"IS_VIDEO", false}
                        };
                        _mediaTemplate.Initialize();
                        sb.Append(_mediaTemplate.TransformText());
                        _latestLink = childNode.Attributes["src"].Value;
                        break;
                    case "iframe":
                        sb.Append(ProcessVideo(childNode));
                        break;
                }
            }
            return sb.ToString();
        }

        private string ProcessVideo(HAP.HtmlNode node)
        {
            var sb = new StringBuilder();
            string sourceUrl = string.Empty;
            string href;

            var url = node.HasAttributes && node.Attributes.Contains("src")
                ? node.Attributes["src"].Value
                : string.Empty;
            if (url.StartsWith("//www"))
                url = url.Replace("//www", "http://www");
            var match = Regex.Match(url, @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string id = match.Groups[1].Value;
                sourceUrl = string.Format("http://img.youtube.com/vi/{0}/0.jpg", id);
                href = string.Format("http://www.youtube.com/watch?v={0}", id);
            }
            else
            {
                //match = Regex.Match(url, @"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)");
                //if (match.Success)
                  //  content.VideoId = match.Groups[1].Value;
                href = url;
            }
            _mediaTemplate.Session = new Dictionary<string, object>
                        {
                            {"HREF", href},
                            {"IMAGE_SOURCE", sourceUrl},
                            {"IS_VIDEO", true}
                        };
            _mediaTemplate.Initialize();
            sb.Append(_mediaTemplate.TransformText());
            _latestLink = href;
            return sb.ToString();
        }
    }
}