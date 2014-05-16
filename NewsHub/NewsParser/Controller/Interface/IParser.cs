using System.Collections.Generic;
using NewsParser.Model;
using NewsParser.Model.Base;
using NewsParser.Model.Events;
using HAP = HtmlAgilityPack;

namespace NewsParser.Controller.Interface
{
    public interface IParser
    {
        event ParseCompleteDelegate ParseComplete;
        void Parse(string uri);
    }
}
