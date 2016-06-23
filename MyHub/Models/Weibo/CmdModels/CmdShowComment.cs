
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 根据微博ID返回某条微博的评论列表 参数类
    /// http://open.weibo.com/wiki/2/comments/show
    /// </summary>
    public class CmdShowComment : ICustomCmdBase
    {
        private string _id = string.Empty;//需要查询的微博ID。 
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

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

        private string _filter_by_author = string.Empty;//作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。  
        public string Filter_by_author
        {
            get { return _filter_by_author; }
            set { _filter_by_author = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/comments/show.json";
            request.Method = Method.GET;

            request.AddParameter("id", Id);
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
            if (Filter_by_author.Length > 0)
            {
                request.AddParameter("filter_by_author", Filter_by_author);
            }
        }
    }
}
