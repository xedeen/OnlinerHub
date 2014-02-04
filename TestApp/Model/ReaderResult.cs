namespace Onliner.Model
{
    public class ReaderResult
    {
        public string content { get; set; }
        public string domain { get; set; }
        public string author { get; set; }
        public string url { get; set; }
        public string short_url { get; set; }
        public string title { get; set; }
        public string excerpt { get; set; }
        public string direction { get; set; }
        public int? word_count { get; set; }
        public string total_pages { get; set; }
        public string date_published { get; set; }
        public string dek { get; set; }
        public string lead_image_url { get; set; }
        public int? next_page_id { get; set; }
        public int? rendered_pages { get; set; }

    }
}
