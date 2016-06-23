
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// http://open.weibo.com/wiki/2/statuses/repost_timeline
    /// </summary>
    public class CmdReportTimeline : ICustomCmdBase
    {
        /// <summary>
        /// 需要查询的微博ID。 必须参数
        /// </summary>
        private string _id = string.Empty;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// 若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。 
        /// </summary>
        private string _since_id = string.Empty;
        public string Since_id
        {
            get { return _since_id; }
            set { _since_id = value; }
        }

        /// <summary>
        /// 若指定此参数，则返回ID小于或等于max_id的微博，默认为0。 
        /// </summary>
        private string _max_id = string.Empty;
        public string Max_id
        {
            get { return this._max_id; }
            set { this._max_id = value; }
        }

        /// <summary>
        /// 单页返回的记录条数，最大不超过200，默认为20。 
        /// </summary>
        private string _count = string.Empty;
        public string Count
        {
            get { return _count; }
            set { _count = value; }
        }

        /// <summary>
        /// 返回结果的页码，默认为1。 
        /// </summary>
        private string _page = string.Empty;
        public string Page
        {
            get { return _page; }
            set { _page = value; }
        }

        /// <summary>
        /// 作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。 
        /// </summary>
        private string _filter_by_author = string.Empty;
        public string Filter_by_author
        {
            get { return _filter_by_author; }
            set { _filter_by_author = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/statuses/repost_timeline.json";
            request.Method = Method.GET;

            request.AddParameter("id", Id);
            if (Since_id.Length > 0)
            {
                request.AddParameter("since_id ", Since_id);
            }
            if (Max_id.Length > 0)
            {
                request.AddParameter("max_id ", Max_id);
            }
            if (Count.Length > 0)
            {
                request.AddParameter("count ", Count);
            }
            if (Page.Length > 0)
            {
                request.AddParameter("page ", Page);
            }
            if (Since_id.Length > 0)
            {
                request.AddParameter("filter_by_author ", Filter_by_author);
            }

        }
    }
}
