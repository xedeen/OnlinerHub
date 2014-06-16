﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using CacheHub.Model;
using CacheHub.Model.Base;
using CacheHub.Model2;

namespace CacheHub
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOnlinerHub" in both code and config file together.
    [ServiceContract]
    public interface IOnlinerHub
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "uri/{articleUrl}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        string GetReadability(string articleUrl);

        [OperationContract]
        [WebInvoke(UriTemplate = "comments/{articleUrl}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        CommentsPageDto GetComments(string articleUrl, int cursor);

        //[OperationContract]
        //[WebInvoke(UriTemplate = "content/{articleUrl}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        //ArticlePageDto GetContent(string articleUrl, int cursor);

        [OperationContract]
        [WebInvoke(UriTemplate = "content/{articleUrl}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        List<ContentItem> GetContentXaml(string articleUrl);

        [OperationContract]
        [WebInvoke(UriTemplate = "header/{articleUrl}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        Header GetHeader(string articleUrl);

        //[OperationContract]
        //[WebInvoke(UriTemplate = "header/{articleUrl}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        //Header GetHeader(string articleUrl);
    }

    public class FullArticlePage
    {
        public List<ContentItem> Article { get; set; }
        public List<CommentDto> Comments { get; set; }
        public Header Header { get; set; }
    }

    [DataContract]
    [KnownType(typeof (P))]
    [KnownType(typeof (Table))]
    [KnownType(typeof(A))]
    [KnownType(typeof(Video))]
    [KnownType(typeof(Image))]
    [KnownType(typeof(TextBlock))]
    public class ArticlePageDto
    {
        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public List<ParagraphBase> paragraphs { get; set; }

        [DataMember]
        public int? previous_page_cursor { get; set; }

        [DataMember]
        public int? next_page_cursor { get; set; }
    }

    [DataContract]
    public class CommentsPageDto
    {
        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public List<CommentDto> comments { get; set; }

        [DataMember]
        public int? previous_page_cursor { get; set; }

        [DataMember]
        public int? next_page_cursor { get; set; }
    }

    [DataContract]
    public class CommentDto
    {

        public int inner_id { get; set; }

        [DataMember]
        public AuthorDto author { get; set; }

        [DataMember]
        public ContentDto content { get; set; }
    }

    [DataContract]
    public class AuthorDto
    {
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string profile_uri { get; set; }

        [DataMember]
        public string avatar_source_uri { get; set; }
    }

    [DataContract]
    public class ContentDto
    {
        [DataMember]
        public List<BlockItemDto> items { get; set; }

        public ContentDto()
        {
            //paragraph_list = new List<ParagraphDto>();
            items = new List<BlockItemDto>();
        }
    }

    [DataContract]
    public class BlockItemDto
    {
        [DataMember]
        public bool is_blockquote { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public List<BlockItemDto> children { get; set; }

        [DataMember]
        public ParagraphDto content { get; set; }

        public BlockItemDto()
        {
            content = new ParagraphDto();
        }
    }

    [DataContract]
    public class ParagraphDto
    {
        [DataMember]
        public List<ParagraphItem> items { get; set; }

        public ParagraphDto()
        {
            items = new List<ParagraphItem>();
        }
    }

    [DataContract]
    public class ParagraphItem
    {
        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string link { get; set; }

        [DataMember]
        public string content { get; set; }
    }
}
