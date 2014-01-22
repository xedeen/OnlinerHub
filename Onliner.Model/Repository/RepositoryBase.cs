using System;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using Onliner.Model.AppModel;

namespace Onliner.Model.Repository
{
    public abstract class RepositoryBase<T, DbT> : IRepository<T>
        where T : Entity
        where DbT : class, IDbEntity, new()
    {
        protected readonly DataClassesDataContext context = new DataClassesDataContext();

        public IQueryable<T> GetAll()
        {
            return GetTable().Select(GetConverter());
        }

        public bool Save(T entity)
        {
            DbT dbEntity;

            if (entity.IsNew())
            {
                dbEntity = new DbT();
            }
            else
            {
                dbEntity = GetTable().Where(x => x.Id.Equals(entity.Id)).SingleOrDefault();
                if (dbEntity == null)
                {
                    return false;
                }
            }

            UpdateEntry(dbEntity, entity);

            if (entity.IsNew())
            {
                GetTable().InsertOnSubmit(dbEntity);
            }

            context.SubmitChanges();

            entity.Id = dbEntity.Id;
            return true;
        }

        public bool Delete(long id)
        {
            var dbEntity = GetTable().Where(x => x.Id.Equals(id)).SingleOrDefault();

            if (dbEntity == null)
            {
                return false;
            }

            GetTable().DeleteOnSubmit(dbEntity);

            context.SubmitChanges();
            return true;
        }

        public bool Delete(T entity)
        {
            return Delete(entity.Id);
        }

        protected abstract Table<DbT> GetTable();
        protected abstract Expression<Func<DbT, T>> GetConverter();
        protected abstract void UpdateEntry(DbT dbEntity, T entity);
    }
}
