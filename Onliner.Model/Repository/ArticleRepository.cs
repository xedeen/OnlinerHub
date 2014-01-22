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
        public static AppModel.Article FindExistingArticle(this IQueryable<AppModel.Article> q, string url)
        {
            return q.FirstOrDefault(a => a.Uri.Equals(url));
        }

        public static IQueryable<AppModel.Article> GetFeedArticles(this IQueryable<AppModel.Article> q, FeedType feedType)
        {
            return q.Where(a => a.FeedType.Equals(feedType));
        }

        public static AppModel.Article Get(this IQueryable<AppModel.Article> q, long id)
        {
            return q.SingleOrDefault(a => a.Id.Equals(id));
        }
    }
    public class ArticleRepository : RepositoryBase<AppModel.Article, Article>
    {
        protected override Table<Article> GetTable()
        {
            return context.Articles;
        }

        protected override Expression<Func<Article, AppModel.Article>> GetConverter()
        {
            return c => new AppModel.Article
                            {
                                Id = c.Id,
                                Title = c.Title,
                                Uri = c.Uri,
                                LastUpdate = c.LastUpdate,
                                FeedType = (FeedType)c.FeedType
                            };

        }
        

        protected override void UpdateEntry(Article dbArticle, AppModel.Article article)
        {
            dbArticle.Title = article.Title;
            dbArticle.Uri = article.Uri;
            dbArticle.LastUpdate = article.LastUpdate;
            dbArticle.FeedType = (int?) article.FeedType;
        }
    }
}
