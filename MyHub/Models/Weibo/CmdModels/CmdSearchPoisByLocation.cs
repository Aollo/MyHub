using WeiboSDKForWinRT;
using RestSharp;


namespace MyHub.Models.Weibo
{
    public class CmdSearchPoisByLocation : ICustomCmdBase
    {
        private string _keyword = string.Empty;
        public string Keyword
        {
            get { return _keyword; }
            set { _keyword = value; }
        }

        private string _count = string.Empty;
        public string Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private string _page = string.Empty;
        public string Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/location/pois/search/by_location.json";
            request.Method = Method.GET;

            if (Keyword.Length > 0)
            {
                request.AddParameter("q", Keyword);
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
