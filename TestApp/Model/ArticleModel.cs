using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;

namespace Onliner.Model
{
    public class ArticleModel
    {
        #region events
        public delegate void ArticleLoadedEventHandler(object sender, ArticleLoadedEventArgs args);

        public class ArticleLoadedError
        {
            public string Message { get; set; }
        }

        public class ArticleLoadedEventArgs
        {
            public ArticleLoadedError Error { get; set; }
            public ArticleModel ArticleModel { get; set; }

            internal ArticleLoadedEventArgs(ArticleLoadedError articleLoadedError, ArticleModel model)
            {
                Error = articleLoadedError;
                ArticleModel = model;
            }
        }

        public event ArticleLoadedEventHandler ArticleLoaded;

        private void NotifyArticleLoaded(ArticleLoadedError error)
        {
            ArticleLoadedEventHandler handler = ArticleLoaded;
            if (null != handler)
            {
                handler(this, new ArticleLoadedEventArgs(error, this));
            }
        }
        #endregion

        #region properties
        public List<global::OnlinerHub.Model.CommentModel> Comments { get; set; }
        #endregion

        #region inner_stuff

        private Uri _currentUri;
        private CookieContainer _cookies;
        private ArticleLoadedError _error;
        private readonly AutoResetEvent _postEvent = new AutoResetEvent(false);
        #endregion

        private byte[] _loginContentBytes = null;
        private byte[] LoginContentBytes
        {
            get
            {
                if (null != _loginContentBytes) return _loginContentBytes;
                var postData = new StringBuilder();
                postData.Append("username=" + HttpUtility.UrlEncode("xedin") + "&");
                postData.Append("password=" + HttpUtility.UrlEncode("spectrum"));
                _loginContentBytes = Encoding.UTF8.GetBytes(postData.ToString());
                return _loginContentBytes;
            }
        }

        public void BeginLoad(string uri)
        {
            try
            {
                _error = null;
                bool useRedirect = false;

                _currentUri = new Uri(uri);
                if (null == _cookies)
                {
                    _cookies = new CookieContainer();
                    useRedirect = true;
                }

                var loginUri = useRedirect
                    ? string.Format("https://profile.onliner.by/login/?redirect={0}", HttpUtility.UrlEncode(uri))
                    : uri;

                var httpWebRequest = (HttpWebRequest) WebRequest.Create(loginUri);
                httpWebRequest.CookieContainer = _cookies;
                httpWebRequest.Method = useRedirect ? "POST" : "GET";
                httpWebRequest.ContentType = useRedirect
                    ? "application/x-www-form-urlencoded"
                    : "text/html; charset=utf-8";

                if (useRedirect)
                {
                    httpWebRequest.ContentLength = LoginContentBytes.Length;
                    httpWebRequest.AllowAutoRedirect = true;
                    httpWebRequest.BeginGetRequestStream(ArticleLoginPostRequestCallback, httpWebRequest);
                    _postEvent.WaitOne();
                }
                httpWebRequest.BeginGetResponse(ArticleResponseCallback, httpWebRequest);
            }
            catch (Exception exception)
            {
                _error = new ArticleLoadedError {Message = exception.Message};
            }
        }

        private void ArticleLoginPostRequestCallback(IAsyncResult asyncResult)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest) asyncResult.AsyncState;
                var postStream = httpWebRequest.EndGetRequestStream(asyncResult);
                postStream.Write(LoginContentBytes, 0, LoginContentBytes.Length);
                postStream.Close();
                _postEvent.Set();
            }
            catch (Exception exception)
            {
                _error = new ArticleLoadedError {Message = exception.Message};
                _postEvent.Set();
            }
        }

        private void ArticleResponseCallback(IAsyncResult asyncResult)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest) asyncResult.AsyncState;
                var response = (HttpWebResponse) httpWebRequest.EndGetResponse(asyncResult);
                var html = new HtmlAgilityPack.HtmlDocument();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    html.LoadHtml(sr.ReadToEnd());
                }
                Deployment.Current.Dispatcher.BeginInvoke(() => ProcessArticle(html));
            }
            catch (Exception exception)
            {
                _error = new ArticleLoadedError {Message = exception.Message};
            }
        }


        private void ProcessArticle(HtmlAgilityPack.HtmlDocument html)
        {
            ProcessPost(html);
            Comments = ProcessComments(html);
            NotifyArticleLoaded(_error);
        }

        private void FixLinks(HtmlAgilityPack.HtmlNode parentNode)
        {
            if (parentNode.Name == "a")
            {
                if (parentNode.HasAttributes && parentNode.Attributes.Contains("href"))
                {
                    var uriName = parentNode.Attributes["href"].Value;
                    Uri uriResult;
                    var result = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult) &&
                                 uriResult.Scheme == Uri.UriSchemeHttp;
                    if (!result)
                    {
                        uriResult = new Uri(_currentUri, uriName);
                        parentNode.Attributes["href"].Value = uriResult.ToString();
                    }

                }
                if ((!parentNode.HasAttributes) || !parentNode.Attributes.Contains("target"))
                    parentNode.Attributes.Add("target", "_blank");
            }
            if (parentNode.HasChildNodes)
            {
                foreach (var node in parentNode.ChildNodes)
                {
                    FixLinks(node);
                }
            }
        }

        private void ProcessPost(HtmlAgilityPack.HtmlDocument html)
        {
            var sb = new StringBuilder();
            var resource = System.Windows.Application.GetResourceStream(new Uri(@"/OnlinerHub;component/Resources/PageCss.txt", UriKind.Relative));

            var streamReader = new StreamReader(resource.Stream);
            sb.Append(streamReader.ReadToEnd());
            
            var node = html.DocumentNode.SelectSingleNode("//article");
            if (null==node) return;

            FixLinks(node);
            
            sb.AppendLine("<article class=\"b-posts-1-item\">");
            sb.AppendLine("<div class=\"article_title\">");

            var comment_icon = node.SelectSingleNode("a[@class='comment-icon-1']");
            if (null != comment_icon)
            {
                sb.AppendLine(comment_icon.OuterHtml);
                node.RemoveChild(comment_icon);
            }

            var postTags = node.SelectSingleNode("div[@class='b-post-tags-1']");
            if (null != postTags)
            {
                sb.AppendLine(postTags.OuterHtml);
                node.RemoveChild(postTags);
            }

            var h3 = node.SelectSingleNode("h3");
            if (null != h3)
            {
                sb.AppendLine(h3.OuterHtml);
                node.RemoveChild(h3);
            }
            sb.AppendLine("</div>");

            foreach (var childNode in node.ChildNodes)
            {

                if (childNode.HasAttributes && childNode.Attributes.Contains("class") &&
                    childNode.Attributes["class"].Value == "social-buttons")
                    continue;
                
                var nodeText = childNode.OuterHtml;
                if (nodeText.Contains("iframe src"))
                    nodeText = nodeText.Replace("<iframe src=\"//", "<iframe src=\"http://");
                sb.AppendLine(nodeText);
            }
            sb.AppendLine("</article>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            var s = sb.ToString();
        }


        private List<global::OnlinerHub.Model.CommentModel> ProcessComments(HtmlAgilityPack.HtmlDocument html)
        {
            try
            {
                var node = html.GetElementbyId("onliner_comments").ChildNodes.FirstOrDefault(x => x.Name == "ul");
                if (node == null) return null;

                var innerId = 0;
                return
                    node.SelectNodes("li[@data-comment-id]")
                        .Select(commentNode => global::OnlinerHub.Model.CommentModel.ParseComment(commentNode, innerId++))
                        .ToList();
            }
            catch (Exception e)
            {
                _error = new ArticleLoadedError {Message = e.Message};
            }
            return new List<global::OnlinerHub.Model.CommentModel>();
        }
    }
}
