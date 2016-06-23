
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// </summary>
    public class CmdTrendsWeekly : ICustomCmdBase
    {
        private string _base_app;//是否只获取当前应用的数据。0为否（所有数据），1为是（仅当前应用），默认为0。
        public string Base_app
        {
            get { return _base_app; }
            set { _base_app = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/trends/weekly.json";
            request.Method = Method.GET;

            if (Base_app.Length > 0)
            {
                request.AddParameter("base_app", Base_app);
            }
        }
    }
}
