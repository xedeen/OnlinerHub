using System.Collections.Generic;
using HAP = HtmlAgilityPack;

namespace CacheHub
{
    public interface IParser
    {
        List<ParagraphBase> Parse(HAP.HtmlDocument html);
    }
}