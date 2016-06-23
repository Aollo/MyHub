
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 获取两个用户之间的共同关注人列表 
    /// http://open.weibo.com/wiki/2/friendships/friends/in_common
    /// </summary>
    public class CmdFriendshipsFriendsInCommon : ICustomCmdBase
    {
        private string _uid = string.Empty;//需要获取共同关注关系的用户UID。
        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        private string _suid = string.Empty;//需要获取共同关注关系的用户UID，默认为当前登录用户。 
        public string Suid
        {
            get { return _suid; }
            set { _suid = value; }
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

        private string _trim_status = string.Empty;//返回值中user字段中的status字段开关，0：返回完整status字段、1：status字段仅返回status_id，默认为1。
        public string Trim_status
        {
            get { return _trim_status; }
            set { _trim_status = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/friendships/friends/in_common.json";
            request.Method = Method.GET;

            request.AddParameter("uid", Uid);

            if (Suid.Length > 0)
            {
                request.AddParameter("suid ", Suid);
            }
            if (Count.Length > 0)
            {
                request.AddParameter("count ", Count);
            }
            if (Page.Length > 0)
            {
                request.AddParameter("page ", Page);
            }
            if (Trim_status.Length > 0)
            {
                request.AddParameter("trim_status ", Trim_status);
            }
        }
    }
}
