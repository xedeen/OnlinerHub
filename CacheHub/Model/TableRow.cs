using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CacheHub.Model
{
    [DataContract]
    public class TableRow
    {
        [DataMember]
        public List<P> Content { get; set; }
    }
}
