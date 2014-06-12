using System.Collections.Generic;
using System.Runtime.Serialization;
using CacheHub.Model.Base;

namespace CacheHub.Model
{
    [DataContract]
    public class P : ParagraphBase
    {
        [DataMember]
        public List<ContentItemBase> Items { get; set; }
    }
}
