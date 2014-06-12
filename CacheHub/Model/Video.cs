using System.Runtime.Serialization;

namespace CacheHub.Model
{
    [DataContract]
    public class Video :Image
    {
        [DataMember]
        public string VideoId { get; set; }
    }
}
