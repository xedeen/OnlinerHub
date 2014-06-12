using System.Runtime.Serialization;
using CacheHub.Model.Base;

namespace CacheHub.Model
{
    [DataContract]
    public class Image : ContentItemBase
    {
        [DataMember]
        public string SourceUrl { get; set; }
        [DataMember]
        public string HRef { get; set; }
    }
}
