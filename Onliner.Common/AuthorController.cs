using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onliner.Model.AppModel;
using Onliner.Model.Repository;

namespace Onliner.Common
{
    public class AuthorController:ControllerBase<AuthorController>
    {
        public IQueryable<Author> GetAllAuthors()
        {
            var repository = new AuthorRepository();
            return repository.GetAll();
        }

        public Author RetrieveAuthor(string profileUri, string name, string avatarUri)
        {
            var authorsRepository = new AuthorRepository();
            var author = authorsRepository.GetAll().FirstOrDefault(
                a =>
                a.ProfileUri.Equals(profileUri) && a.Name.Equals(name));
            if (null == author)
            {
                author = new Author
                             {
                                 AvatarUri = avatarUri,
                                 Name = name,
                                 ProfileUri = profileUri
                             };
                authorsRepository.Save(author);
            }
            return author;
        }
    }
}
