using System.Collections.Generic;
namespace NewsParser.Model
{
    public class Header
    {   
        public string Title { get; set; }
        public List<Tag> Tags { get; set; }
        public Image Image { get; set; }
        public string Error { get; set; }
    }
}
