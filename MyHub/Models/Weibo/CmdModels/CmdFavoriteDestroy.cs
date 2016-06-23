
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdFavoriteDestroy : ICustomCmdBase
    {
        private string _id = string.Empty;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/favorites/destroy.json";
            request.Method = Method.POST;

            request.AddParameter("id", Id);
        }
    }
}
