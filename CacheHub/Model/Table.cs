using System.Collections.Generic;
using System.Runtime.Serialization;
using CacheHub.Model.Base;

namespace CacheHub.Model
{
    [DataContract]
    public class Table : ParagraphBase
    {
        [DataMember]
        public List<TableRow> Rows { get; set; }
    }
}
