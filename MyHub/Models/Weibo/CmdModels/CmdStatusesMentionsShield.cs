
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 屏蔽某个@到我的微博以及后续由对其转发引起的@提及 
    /// http://open.weibo.com/wiki/2/statuses/mentions/shield
    /// </summary>
    public class CmdStatusesMentionsShield : ICustomCmdBase
    {
        private string _id = string.Empty;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _follow_up = string.Empty;
        public string Follow_up
        {
            get { return _follow_up; }
            set { _follow_up = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/statuses/mentions/shield.json";
            request.Method = Method.POST;

            request.AddParameter("id", Id);
            if (Follow_up.Length > 0)
            {
                request.AddParameter("follow_up", Follow_up);
            }
        }
    }
}
