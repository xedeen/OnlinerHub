using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CacheHub
{

    [DataContract]
    public class Article
    {
        public Header Header { get; set; }
        public List<ParagraphBase> Content { get; set; }
    }

    [DataContract]
    public class Header
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public List<Tag> Tags { get; set; }
        [DataMember]
        public Image Image { get; set; }

        [DataMember]
        public string Error { get; set; }

    }

    [DataContract]
    public class Tag
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public TagSize Size { get; set; }
        [DataMember]
        public string HRef { get; set; }
    }

    [DataContract]
    public class ParagraphBase
    {
        public int InnerId { get; set; }
    }

    [DataContract]
    public class Table : ParagraphBase
    {
        [DataMember]
        public List<TableRow> Rows { get; set; }
    }

    [DataContract]
    public class TableRow
    {
        [DataMember]
        public List<P> Content { get; set; }
    }

    [DataContract]
    public class P : ParagraphBase
    {
        [DataMember]
        public List<BaseContentItem> Items { get; set; }
    }

    [DataContract]
    public class BaseContentItem
    {
        [DataMember]
        public string Text { get; set; }
    }

    [DataContract]
    public class A : BaseContentItem
    {
        [DataMember]
        public string HRef { get; set; }
    }

    [DataContract]
    public class Video : Image
    {
        [DataMember]
        public string VideoId { get; set; }
    }

    [DataContract]
    public class Image : BaseContentItem
    {
        [DataMember]
        public string SourceUrl { get; set; }
        [DataMember]
        public string HRef { get; set; }
    }

    [DataContract]
    public class TextBlock : BaseContentItem
    {
        [DataMember]
        public int TextModifiers { get; set; }
    }

    public enum TagSize
    {
        Small = 0,
        Big = 1
    }

    [Flags]
    public enum TextModifiers
    {
        None = 0x0000,
        Bold = 0x0001,
        Italic = 0x0010,
        Underline = 0x0100
    }
}