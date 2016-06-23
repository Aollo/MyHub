
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 获取用户的粉丝列表 参数类
    /// 参数uid与screen_name二者必选其一，且只能选其一；最多返回5000条数据； 
    /// http://open.weibo.com/wiki/2/friendships/followers
    /// </summary>
    public class CmdFriendshipsFollowers : ICustomCmdBase
    {
        private string _screen_name = string.Empty;//需要查询的用户昵称。 
        public string Screen_name
        {
            get { return _screen_name; }
            set { _screen_name = value; }
        }

        private string _count = string.Empty;//单页返回的记录条数，默认为50，最大不超过200。
        public string Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private string _cursor = string.Empty;//返回结果的游标，下一页用返回值里的next_cursor，上一页用previous_cursor，默认为0。
        public string Cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }

        private string _trim_status = string.Empty;//返回值中user字段中的status字段开关，0：返回完整status字段、1：status字段仅返回status_id，默认为1。
        public string Trim_status
        {
            get { return _trim_status; }
            set { _trim_status = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/friendships/followers.json";
            request.Method = Method.GET;

            request.AddParameter("screen_name", Screen_name);
            if (Count.Length > 0)
            {
                request.AddParameter("count", Count);
            }
            if (Cursor.Length > 0)
            {
                request.AddParameter("cursor", Cursor);
            }
            if (Trim_status.Length > 0)
            {
                request.AddParameter("trim_status", Trim_status);
            }
        }
    }
}
