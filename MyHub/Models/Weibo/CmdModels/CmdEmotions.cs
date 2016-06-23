
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 获取微博官方表情的详细信息 
    /// http://open.weibo.com/wiki/2/emotions
    /// </summary>
    public class CmdEmotions : ICustomCmdBase
    {
        private string _type = string.Empty;//表情类别，face：普通表情、ani：魔法表情、cartoon：动漫表情，默认为face。 
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _language = string.Empty;//语言类别，cnname：简体、twname：繁体，默认为cnname。
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/emotions.json";
            request.Method = Method.GET;

            if (Type.Length > 0)
            {
                request.AddParameter("type", Type);
            }
            if (Language.Length > 0)
            {
                request.AddParameter("language", Language);
            }
        }
    }
}
