
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdRemindUnreadCount : ICustomCmdBase
    {
        private string _uid = string.Empty;//若指定此参数，则返回ID比since_id大的评论（即比since_id时间晚的评论），默认为0。
        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/remind/unread_count.json";
            request.Method = Method.GET;

            if (Uid.Length > 0)
            {
                request.AddParameter("uid", Uid);
            }
        }
    }
}
