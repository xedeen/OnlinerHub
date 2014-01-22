using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Onliner.Model.AppModel;
using Onliner.Model.Repository;
using HAP = HtmlAgilityPack;

namespace Onliner.Common
{
    public class ArticleController : ControllerBase<ArticleController>
    {
        public class ReaderResult
        {
            public string content { get; set; }
            public string domain { get; set; }
            public string author { get; set; }
            public string url { get; set; }
            public string short_url { get; set; }
            public string title { get; set; }
            public string excerpt { get; set; }
            public string direction { get; set; }
            public int? word_count { get; set; }
            public string total_pages { get; set; }
            public string date_published { get; set; }
            public string dek { get; set; }
            public string lead_image_url { get; set; }
            public int? next_page_id { get; set; }
            public int? rendered_pages { get; set; }
        }

        public Article GetArticle(long id)
        {
            var repository = new ArticleRepository();
            return repository.GetAll().FirstOrDefault(a => a.Id.Equals(id));
        }

        public void ProcessArticle(Article article)
        {
            ContainerController.Instance.PushContent(article);
            ProcessComments(article, false);
            SetArticleLastUpdate(article);
        }

        public void ProcessComments(Article article, bool setLastUpdate=true)
        {
            try
            {
                CommentsController.Instance.PushComments(article.Id);
                if (setLastUpdate)
                    SetArticleLastUpdate(article);
            }
            catch{}


            
        }

        private void SetArticleLastUpdate(Article article)
        {
            article.LastUpdate = DateTime.Now;
            var repository = new ArticleRepository();
            repository.Save(article);
        }
    }
}
