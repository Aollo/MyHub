
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 根据微博ID删除指定微博 参数类
    /// http://open.weibo.com/wiki/2/statuses/destroy
    /// </summary>
    public class CmdStatusesDestroy : ICustomCmdBase
    {
        private string _id = string.Empty;//需要删除的微博ID。
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/statuses/destroy.json";
            request.Method = Method.POST;

            request.AddParameter("id", Id);
        }
    }
}
