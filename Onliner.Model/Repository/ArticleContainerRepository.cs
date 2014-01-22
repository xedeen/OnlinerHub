using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.Repository
{
    public static partial class RepositoryExtension
    {
        public static AppModel.ArticleContainer GetContent(this IQueryable<AppModel.ArticleContainer> q, AppModel.Article article)
        {
            return q.FirstOrDefault(c => c.ArticleId.Equals(article.Id));
        }
    }

    public class ArticleContainerRepository : RepositoryBase<AppModel.ArticleContainer, ArticleContainer>
    {
        protected override Table<ArticleContainer> GetTable()
        {
            return context.ArticleContainers;
        }

        protected override Expression<Func<ArticleContainer, AppModel.ArticleContainer>> GetConverter()
        {
            return a => new AppModel.ArticleContainer
            {
                Id = a.Id,
                ArticleId = a.ArticleId,
                Representation = a.Representation
            };
        }

        protected override void UpdateEntry(ArticleContainer dbContainer, AppModel.ArticleContainer container)
        {
            dbContainer.ArticleId=container.ArticleId;
            dbContainer.Representation = container.Representation;
        }
    }
}
