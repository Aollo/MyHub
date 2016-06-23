using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdLocalPublicTimelines : ICustomCmdBase
    {
        private string _longitude = string.Empty;//若指定此参数，则返回ID比since_id大的评论（即比since_id时间晚的评论），默认为0。
        public string Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        private string _latitude = string.Empty;//若指定此参数，则返回ID小于或等于max_id的评论，默认为0。 
        public string Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
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

        private string _range = string.Empty;//作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。  
        public string Range
        {
            get { return _range; }
            set { _range = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/place/nearby_timeline.json";
            request.Method = Method.GET;

            if (Longitude.Length > 0)
            {
                request.AddParameter("long", Longitude);
            }
            if (Latitude.Length > 0)
            {
                request.AddParameter("lat", Latitude);
            }
            if (Count.Length > 0)
            {
                request.AddParameter("count", Count);
            }
            if (Page.Length > 0)
            {
                request.AddParameter("page", Page);
            }
            if (Range.Length > 0)
            {
                request.AddParameter("range", Range);
            }
        }
    }
}
