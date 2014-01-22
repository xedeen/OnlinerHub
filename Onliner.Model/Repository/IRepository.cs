using System.Linq;
using Onliner.Model.AppModel;

namespace Onliner.Model.Repository
{
    public interface IRepository<T> where T : Entity
    {
        IQueryable<T> GetAll();
        bool Save(T entity);
        bool Delete(long id);
        bool Delete(T entity);
    }
}
