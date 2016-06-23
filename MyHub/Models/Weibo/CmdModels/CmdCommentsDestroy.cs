
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 删除一条评论 
    /// http://open.weibo.com/wiki/2/comments/destroy
    /// </summary>
    public class CmdCommentsDestroy : ICustomCmdBase
    {
        private string _cid;//要删除的评论ID，只能删除登录用户自己发布的评论。
        public string Cid
        {
            get { return _cid; }
            set { _cid = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/comments/destroy.json";
            request.Method = Method.POST;

            request.AddParameter("cid", Cid);
        }
    }
}
