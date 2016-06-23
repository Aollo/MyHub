using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    public class CmdUserHomeTimeline : ICustomCmdBase
    {
        private string _since_id = string.Empty;//若指定此参数，则返回ID比since_id大的评论（即比since_id时间晚的评论），默认为0。
        public string Since_id
        {
            get { return _since_id; }
            set { _since_id = value; }
        }

        private string _max_id = string.Empty;//若指定此参数，则返回ID小于或等于max_id的评论，默认为0。 
        public string Max_id
        {
            get { return _max_id; }
            set { _max_id = value; }
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

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/statuses/home_timeline.json";
            request.Method = Method.GET;

            if (Since_id.Length > 0)
            {
                request.AddParameter("since_id", Since_id);
            }
            if (Max_id.Length > 0)
            {
                request.AddParameter("max_id", Max_id);
            }
            if (Count.Length > 0)
            {
                request.AddParameter("count", Count);
            }
            if (Page.Length > 0)
            {
                request.AddParameter("page", Page);
            }
        }
    }
}
