using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Onliner.Model;
using Onliner.Model.Repository;
using Article = Onliner.Model.AppModel.Article;

namespace Onliner.Common
{
    public class FeedController:ControllerBase<FeedController>
    { 
        public List<Article> GetRss(FeedType feedType)
        {   
            Task.Run(() => RetrieveRss(feedType));
            var repository = new ArticleRepository();
            return repository.GetAll().GetFeedArticles(feedType).ToList();
        }

        private void RetrieveRss(FeedType feedType)
        {
            var feedUrl = GetFeedUrl(feedType);
            try
            {
                using (var reader = XmlReader.Create(feedUrl))
                {
                    var feed = SyndicationFeed.Load(reader);
                    if (null == feed) return;

                    var repository = new ArticleRepository();
                    foreach (
                        var article in
                            feed.Items.Select(
                                item => repository.GetAll().FindExistingArticle(item.Links[0].Uri.ToString()) ??
                                        new Article
                                            {
                                                Uri = item.Links[0].Uri.ToString(),
                                                Title = item.Title.Text,
                                                FeedType = feedType
                                            }))
                    {
                        repository.Save(article);

                        if (null == article.LastUpdate) //prepare full update async
                        {
                            var article1 = article;
                            Task.Run(() => ArticleController.Instance.ProcessArticle(article1));
                        }
                        else if ((DateTime.Now - article.LastUpdate.Value).TotalMinutes > 10)
                        {
                            var article1 = article;
                            Task.Run(() => ArticleController.Instance.ProcessComments(article1));
                        }
                    }
                }
            }
            catch
            {
            }
        }

        

        private string GetFeedUrl(FeedType feedType)
        {
            switch (feedType)
            {
                case FeedType.Auto:
                    return "http://auto.onliner.by/feed";
                case FeedType.Tech:
                    return "http://tech.onliner.by/feed";
                case FeedType.People:
                    return "http://people.onliner.by/feed";
                case FeedType.Realt:
                    return "http://realt.onliner.by/feed";
            }
            return string.Empty;
        }

    }
}
