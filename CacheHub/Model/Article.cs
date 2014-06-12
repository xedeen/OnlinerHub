using System.Collections.Generic;
using System.Runtime.Serialization;
using CacheHub.Model.Base;

namespace CacheHub.Model
{
    [DataContract]
    public class Article
    {
        [DataMember]
        public long PostId { get; set; }
        [DataMember]
        public Header Header { get; set; }
        [DataMember]
        public List<ParagraphBase> Content { get; set; }
        [DataMember]
        public List<Comment> Comments { get; set; }
    }
}
