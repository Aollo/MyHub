
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 屏蔽某条微博 ,高级接口（需要授权）
    /// http://open.weibo.com/wiki/2/statuses/filter/create
    /// </summary>
    public class CmdStatusesFilterCreate : ICustomCmdBase
    {
        private string _id = string.Empty;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/statuses/filter/create.json";
            request.Method = Method.POST;

            request.AddParameter("id", Id);
        }
    }
}
