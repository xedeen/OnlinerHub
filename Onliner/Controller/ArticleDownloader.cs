using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Onliner.Contracts;
using Onliner.OnlinerHub;

namespace Onliner.Controller
{
    public class ArticleDownloader
    {
        public void StartRetrieveArticle(string uri)
        {
            try
            {
                var requestUrl =
                    string.Format(
                        "https://www.readability.com/api/content/v1/parser?url={0}&token=ecd1f4e3683b5bfbd41c02c20229011983d03671",
                        uri);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.BeginGetResponse(ArticleDownload_Completed, httpWebRequest);
            }
            catch (Exception e)
            {
            }
        }

        private void ArticleDownload_Completed(IAsyncResult result)
        {
            var request = (HttpWebRequest) result.AsyncState;
            var response = (HttpWebResponse) request.EndGetResponse(result);
            var serializer = new DataContractJsonSerializer(typeof(ReadabilityResult));
            ReadabilityResult current;

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                current = (ReadabilityResult) serializer.ReadObject(sr.BaseStream);
            }
            var text =
                "<html><head><style type=\"text/css\">body {background-color: black ;color:white} </style></head><body>" +
                current.content + "</body></html>";
        }
    }
}
