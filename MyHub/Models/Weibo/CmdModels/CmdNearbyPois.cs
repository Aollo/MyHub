using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdNearbyPois : ICustomCmdBase
    {
        private string _latitude = string.Empty;
        public string Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        private string _longitude = string.Empty;
        public string Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        private string _range = string.Empty;
        public string Range
        {
            get { return _range; }
            set { _range = value; }
        }

        private string _keyword = string.Empty;
        public string Keyword
        {
            get { return _keyword; }
            set { _keyword = value; }
        }

        private string _count = string.Empty;
        public string Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private string _page = string.Empty;
        public string Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/place/nearby/pois.json";
            request.Method = Method.GET;

            if (Latitude.Length > 0)
            {
                request.AddParameter("lat", Latitude);
            }
            if (Longitude.Length > 0)
            {
                request.AddParameter("long", Longitude);
            }
            if (Keyword.Length > 0)
            {
                request.AddParameter("q", Keyword);
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
