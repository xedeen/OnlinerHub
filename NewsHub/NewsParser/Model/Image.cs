using NewsParser.Model.Base;

namespace NewsParser.Model
{
    public class Image : ContentItemBase
    {
        public string SourceUrl { get; set; }
        public string HRef { get; set; }
    }
}
