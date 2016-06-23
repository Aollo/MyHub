
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdRemindClearCount : ICustomCmdBase
    {
        private string _type = string.Empty;
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string[] Remind_Type = { "follower", "cmt", "dm", "mention_status", "mention_cmt", "group", "notice", "invite", "badge", "photo"};
        public enum REMIND_TYPE_INDEX{ follower = 0, cmt, dm, mention_status , mention_cmt , group , notice , invite , badge , photo };

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/remind/set_count.json";
            request.Method = Method.POST;

            if (Type.Length > 0)
            {
                request.AddParameter("type", Type);
            }
        }
    }
}
