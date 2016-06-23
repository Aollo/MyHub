using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdSearchStatusWithTopic : ICustomCmdBase
    {
        private string _topic = string.Empty; 
        public string Topic
        {
            get { return _topic; }
            set { _topic = value; }
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
            request.Resource = "/search/topics.json";
            request.Method = Method.GET;

            if (Topic.Length > 0)
            {
                request.AddParameter("q", Topic);
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
