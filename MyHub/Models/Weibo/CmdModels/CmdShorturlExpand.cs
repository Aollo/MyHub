
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 将一个或多个短链接还原成原始的长链接 
    /// http://open.weibo.com/wiki/2/short_url/expand
    /// </summary>
    public class CmdShorturlExpand : ICustomCmdBase
    {
        private string _url_short = string.Empty;
        public string Url_short
        {
            get { return _url_short; }
            set { _url_short = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/short_url/expand.json";
            request.Method = Method.GET;

            request.AddParameter("url_short", Url_short);
        }
    }
}
