using System.Runtime.Serialization;
using CacheHub.Model.Base;

namespace CacheHub.Model
{
    [DataContract]
    public class A : ContentItemBase
    {
        [DataMember]
        public string HRef { get; set; }
    }
}
