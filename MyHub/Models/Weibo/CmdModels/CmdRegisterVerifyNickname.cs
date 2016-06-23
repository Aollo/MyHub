
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 验证昵称是否可用，并给予建议昵称 参数类
    /// http://open.weibo.com/wiki/2/register/verify_nickname
    /// </summary>
    public class CmdRegisterVerifyNickname : ICustomCmdBase
    {
        private string _nickname = string.Empty;
        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/register/verify_nickname.json";
            request.Method = Method.GET;

            request.AddParameter("nickname", Nickname);
        }
    }
}
