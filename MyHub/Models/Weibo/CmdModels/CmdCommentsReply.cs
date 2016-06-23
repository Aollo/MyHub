
using WeiboSDKForWinRT;
using RestSharp;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 回复一条评论 
    /// http://open.weibo.com/wiki/2/comments/reply
    /// </summary>
    public class CmdCommentsReply : ICustomCmdBase
    {
        private string _cid = string.Empty;//需要回复的评论ID。
        public string Cid
        {
            get { return _cid; }
            set { _cid = value; }
        }

        private string _id = string.Empty;//需要评论的微博ID。 
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _comment = string.Empty;//评论内容，必须做URLencode，内容不超过140个汉字。 
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        private string _without_mention = string.Empty;//回复中是否自动加入“回复@用户名”，0：是、1：否，默认为0。 
        public string Without_mention
        {
            get { return _without_mention; }
            set { _without_mention = value; }
        }

        private string _comment_ori = string.Empty;//当评论转发微博时，是否评论给原微博，0：否、1：是，默认为0。 
        public string Comment_ori
        {
            get { return _comment_ori; }
            set { _comment_ori = value; }
        }

        private string _rip = string.Empty;//开发者上报的操作用户真实IP，形如：211.156.0.1。
        public string Rip
        {
            get { return _rip; }
            set { _rip = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/comments/reply.json";
            request.Method = Method.POST;

            request.AddParameter("cid", Cid);
            request.AddParameter("comment", Comment);
            request.AddParameter("id", Id);

            if (Without_mention.Length > 0)
            {
                request.AddParameter("without_mention", Without_mention);
            }
            if (Comment_ori.Length > 0)
            {
                request.AddParameter("comment_ori", Comment_ori);
            }
            if (Rip.Length > 0)
            {
                request.AddParameter("rip", Rip);
            }
        }
    }
}
