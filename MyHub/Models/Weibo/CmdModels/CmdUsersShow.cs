
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 根据用户ID获取用户信息参数类。参数uid与screen_name二者必选其一，且只能选其一 。
    /// http://open.weibo.com/wiki/2/users/show
    /// </summary>
    public class CmdUsersShow : ICustomCmdBase
    {
        private string _screen_name = string.Empty;//需要查询的用户昵称。
        public string Screen_name
        {
            get { return _screen_name; }
            set { _screen_name = value; }
        }

        private string _uid;//需要查询的用户ID。
        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/users/show.json";
            request.Method = Method.GET;

            if(Uid != null && Uid.Length > 0)
            {
                request.AddParameter("uid", Uid);
            }
            else if (Screen_name != null && Screen_name.Length > 0)
            {
                request.AddParameter("screen_name", Screen_name);
            }
        }
    }
}
