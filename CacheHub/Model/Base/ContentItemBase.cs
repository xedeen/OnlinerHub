using System.Runtime.Serialization;

namespace CacheHub.Model.Base
{
    [DataContract]
    public class ContentItemBase
    {
        [DataMember]
        public string Text { get; set; }
    }
}
