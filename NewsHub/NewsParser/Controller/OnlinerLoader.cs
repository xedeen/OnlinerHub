using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NewsParser.Controller.Interface;
using NewsParser.Model.Events;

namespace NewsParser.Controller
{
    public class GetReguestResult
    {
        public CookieCollection Cookies { get; set; }
        public HtmlDocument Doc { get; set; }
    }
    public class OnlinerLoader : ILoader
    {
        private string _userName;
        private string _password;
        private string _uri;

        public event LoadCompleteDelegate LoadComplete;

        public OnlinerLoader(string name, string password)
        {
            _userName = name;
            _password = password;
        }

        public void Load(string uri)
        {
            _uri = uri;
            Task.Factory.StartNew(LoadAsync);
        }

        private async void LoadAsync()
        {
            var success = true;
            GetReguestResult result = null;
            try
            {
                //var request = (HttpWebRequest) WebRequest.Create(_uri);
                //request.Method = "GET";
                //request.ContentType = "text/html; charset=utf-8";
                //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
                //result = await get(request);
                var profileUri = new Uri(string.Format("https://profile.onliner.by/login/?redirect={0}",
                    HttpUtility.UrlEncode(_uri)));

                var request = (HttpWebRequest) WebRequest.Create(profileUri);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
                request.CookieContainer = new CookieContainer();
                //request.CookieContainer.Add(profileUri, result.Cookies);
                result = await post(request, "charset=utf-8&username=" + _userName + "&password=" + _password);
            }
            catch (Exception e)
            {
                success = false;
            }

            if (null != LoadComplete)
            {
                LoadComplete.Invoke(this, new LoadCompleteEventArgs
                {
                    Page = null != result ? result.Doc : null,
                    Success = success
                });
            }
        }



        private async Task<GetReguestResult> post(HttpWebRequest request, string postdata)
        {
            byte[] data = Encoding.UTF8.GetBytes(postdata);
            request.ContentLength = data.Length;

            using (var requestStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request))
            {
                await requestStream.WriteAsync(data, 0, data.Length);
            }
            return await get(request);
        }

        private async Task<GetReguestResult> get(HttpWebRequest request)
        {
            WebResponse responseObject = await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, request);
            var responseStream = responseObject.GetResponseStream();
            var doc = new HtmlDocument();
            doc.Load(responseStream);
            return new GetReguestResult {Cookies = null, Doc = doc};
            //var sr = new StreamReader(responseStream);
            //string received = await sr.ReadToEndAsync();
            //return received;
        }
    }
}
