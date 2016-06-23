using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdFavoriteCreate : ICustomCmdBase
    {
        private string _id = string.Empty;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/favorites/create.json";
            request.Method = Method.POST;

            request.AddParameter("id", Id);
        }
    }
}
