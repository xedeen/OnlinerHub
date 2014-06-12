using CacheHub.Model.Base;

namespace CacheHub.Model
{
    public class CommentAuthor : ParagraphBase
    {
        public string ProfileUrl { get; set; }
        public string AvatarSourceUrl { get; set; }
        public string Name { get; set; }
    }
}
