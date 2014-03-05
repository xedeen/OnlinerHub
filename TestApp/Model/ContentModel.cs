using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HAP = HtmlAgilityPack;

namespace OnlinerHub.Model
{
    public class ContentModel
    {   
        public List<BlockItemModel> items { get; set; }

        public ContentModel()
        {   
            items = new List<BlockItemModel>();
        }

        public static ContentModel ParseContent(HAP.HtmlNode content)
        {
            var currentContent = new ContentModel();
            var currentFormatting = string.Empty;
            ProcessNode(content, null, currentContent, currentFormatting);
            return currentContent;
        }

        private static void ProcessNode(HAP.HtmlNode node, BlockItemModel currentBlock, ContentModel commentContent, string currentFormatting)
        {
            foreach (var child in node.ChildNodes)
            {
                if (currentBlock == null)
                {
                    currentBlock = new BlockItemModel {content = new ParagraphModel()};
                    commentContent.items.Add(currentBlock);
                }

                switch (child.Name)
                {
                    case "blockquote":
                        var newBlock = new BlockItemModel { is_blockquote = true };
                        ProcessNode(child.SelectSingleNode("div"), newBlock, commentContent, currentFormatting);
                        if (currentBlock.is_blockquote)
                        {
                            if (null == currentBlock.children)
                                currentBlock.children = new List<BlockItemModel>();
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
                            currentBlock.content.items.Add(new ParagraphItemModel
                            {
                                content = text,
                                type = currentFormatting,
                                link = child.Attributes["href"].Value
                            });
                        break;
                }
            }
        }

        private static string ProcessBlockItem(HAP.HtmlNode node, BlockItemModel currentBlock, string currentFormatting, string format = null)
        {
            var text = node.InnerText.Trim('\n').Trim();
            if (!string.IsNullOrEmpty(format) && !currentFormatting.Contains(format))
                currentFormatting += format;

            if (!string.IsNullOrEmpty(text))
            {
                currentBlock.content.items.Add(new ParagraphItemModel
                {
                    content = text,
                    type = currentFormatting
                });
            }
            return currentFormatting;
        }
    }
}
