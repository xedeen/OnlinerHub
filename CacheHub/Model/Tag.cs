using System.Runtime.Serialization;

namespace CacheHub.Model
{
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
}
