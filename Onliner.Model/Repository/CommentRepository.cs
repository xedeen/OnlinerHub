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
        public static IQueryable<AppModel.Comment> CommentsOf(this IQueryable<AppModel.Comment> q,
                                                              long articleId)
        {
            return q.Where(c => c.ArticleId.Equals(articleId));
        }
    }

    public class CommentRepository : RepositoryBase<AppModel.Comment, Comment>
    {
        protected override Table<Comment> GetTable()
        {
            return context.Comments;
        }

        protected override Expression<Func<Comment, AppModel.Comment>> GetConverter()
        {
            return c => new AppModel.Comment
                            {
                                Id = c.Id,
                                ArticleId = c.ArticleId,
                                AuthorId = c.AuthorId,
                                CommentContent = c.CommentContent
                            };
        }

        protected override void UpdateEntry(Comment dbEntity, AppModel.Comment entity)
        {
            dbEntity.ArticleId = entity.ArticleId;
            dbEntity.AuthorId = entity.AuthorId;
            dbEntity.CommentContent = entity.CommentContent;
        }
    }
}
