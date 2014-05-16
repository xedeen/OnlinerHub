using NewsParser.Model.Events;

namespace NewsParser.Controller.Interface
{
    public interface ILoader
    {
        event LoadCompleteDelegate LoadComplete;
        void Load(string uri);
    }
}
