using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CacheHub.Model2
{
    public enum ContentType
    {
        Null,
        TextBlock,
        A,
        Image,
        Video
    }
    public enum ContentItemType
    {
        Header,
        Content,
        Footer
    }

    [Flags]
    public enum TextModifiers
    {
        None = 0x0000,
        Bold = 0x0001,
        Italic = 0x0010,
        Underline = 0x0100
    }

    [DataContract]
    public class ContentItem
    {
        public int InnerId { get; set; }
        [DataMember]
        public ContentItemType ContentType { get; set; }
        [DataMember]
        public string Xaml { get; set; }
    }

}