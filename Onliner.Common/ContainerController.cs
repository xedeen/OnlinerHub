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

namespace Onliner.Common
{
    public class ContainerController : ControllerBase<ContainerController>
    {
        public void PushContent(Article article)
        {
            RetrieveContent(article);
        }

        public ArticleContainer RetrieveContent(Article article)
        {
            var contentRepository = new ArticleContainerRepository();
            var contentItem = contentRepository.GetAll().GetContent(article);
            if (null == contentItem)
            {
                try
                {

                    var requestUrl =
                        string.Format(
                            "https://www.readability.com/api/content/v1/parser?url={0}&token=ecd1f4e3683b5bfbd41c02c20229011983d03671",
                            article.Uri);

                    var httpWebRequest = (HttpWebRequest) WebRequest.Create(requestUrl);
                    httpWebRequest.Method = WebRequestMethods.Http.Get;
                    httpWebRequest.Accept = "application/json";

                    var response = httpWebRequest.GetResponse();
                    var serializer = new DataContractJsonSerializer(typeof (ArticleController.ReaderResult));

                    ArticleController.ReaderResult result;
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = (ArticleController.ReaderResult) serializer.ReadObject(sr.BaseStream);
                    }

                    if (result != null && result.content != null)
                    {
                        contentRepository.Save(new ArticleContainer
                                                   {
                                                       ArticleId = article.Id,
                                                       Representation = result.content
                                                   });
                    }
                }
                catch (Exception e)
                {
                }
            }
            return contentItem;
        }
    }
}
