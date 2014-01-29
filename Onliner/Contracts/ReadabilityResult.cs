using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Contracts
{
    [DataContract]
    class ReadabilityResult
    {
        [DataMember]
        public string content { get; set; }
        [DataMember]
        public string domain { get; set; }
        [DataMember]
        public string author { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string short_url { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string excerpt { get; set; }
        [DataMember]
        public string direction { get; set; }
        [DataMember]
        public int? word_count { get; set; }
        [DataMember]
        public string total_pages { get; set; }
        [DataMember]
        public string date_published { get; set; }
        [DataMember]
        public string dek { get; set; }
        [DataMember]
        public string lead_image_url { get; set; }
        [DataMember]
        public int? next_page_id { get; set; }
        [DataMember]
        public int? rendered_pages { get; set; }
    }
}
