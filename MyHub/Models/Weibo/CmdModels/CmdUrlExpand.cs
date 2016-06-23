using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdUrlExpand : ICustomCmdBase
    {
        public string UrlShort { get; set; }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/short_url/expand.json";
            request.Method = Method.GET;

            if (UrlShort.Length > 0)
            {
                request.AddParameter("url_short", UrlShort);
            }
        }
    }
}
