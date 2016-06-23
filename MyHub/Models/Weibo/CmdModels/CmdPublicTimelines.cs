using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdPublicTimelines : ICustomCmdBase
    {
        private string _base_app = string.Empty; 
        public string BaseApp
        {
            get { return _base_app; }
            set { _base_app = value; }
        }

        private string _count = string.Empty;//单页返回的记录条数，默认为50。 
        public string Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private string _page = string.Empty;//返回结果的页码，默认为1。 
        public string Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/statuses/public_timeline.json";
            request.Method = Method.GET;

            if (BaseApp.Length > 0)
            {
                request.AddParameter("base_app", BaseApp);
            }
            if (Count.Length > 0)
            {
                request.AddParameter("count", Count);
            }
            if (Page.Length > 0)
            {
                request.AddParameter("page", Page);
            }
        }
    }
}
