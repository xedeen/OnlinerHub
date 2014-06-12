using System.Collections.Generic;
using CacheHub.Model.Base;
using HAP = HtmlAgilityPack;

namespace CacheHub
{
    public interface IParser
    {
        List<ParagraphBase> Parse(HAP.HtmlDocument html);
    }
}