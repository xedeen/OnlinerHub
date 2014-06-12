using System.Runtime.Serialization;
using CacheHub.Model.Base;

namespace CacheHub.Model
{
    [DataContract]
    public class TextBlock : ContentItemBase
    {
        [DataMember]
        public int TextModifiers { get; set; }
    }
}
