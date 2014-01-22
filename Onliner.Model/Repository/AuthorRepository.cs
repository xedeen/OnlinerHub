using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.Repository
{
    public class AuthorRepository : RepositoryBase<AppModel.Author, Author>
    {
        protected override Table<Author> GetTable()
        {
            return context.Authors;
        }

        protected override Expression<Func<Author, AppModel.Author>> GetConverter()
        {
            return a => new AppModel.Author
            {
                Id = a.Id,
                AvatarUri = a.AvatarUri,
                Name = a.Name,
                ProfileUri = a.ProfileUri
            };
        }

        protected override void UpdateEntry(Author dbAuthor, AppModel.Author author)
        {
            dbAuthor.AvatarUri = author.AvatarUri;
            dbAuthor.Name = author.Name;
            dbAuthor.ProfileUri = author.ProfileUri;
        }
    }
}
