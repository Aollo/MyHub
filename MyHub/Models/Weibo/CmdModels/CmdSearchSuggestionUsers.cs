
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdSearchSuggestionUsers : ICustomCmdBase
    {
        private string _keyword = string.Empty;//单页返回的记录条数，默认为50。 
        public string Keyword
        {
            get { return _keyword; }
            set { _keyword = value; }
        }

        private string _count = string.Empty;//单页返回的记录条数，默认为10。 
        public string Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/suggestions/users.json";
            request.Method = Method.GET;

            if (Keyword.Length > 0)
            {
                request.AddParameter("q", Keyword);
            }
            if (Count.Length > 0)
            {
                request.AddParameter("count", Count);
            }
        }
    }
}
