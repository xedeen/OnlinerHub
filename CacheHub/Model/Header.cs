using System.Collections.Generic;
using System.Runtime.Serialization;
using CacheHub.Model.Base;

namespace CacheHub.Model
{
    [DataContract]
    public class Header : ParagraphBase
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
}
