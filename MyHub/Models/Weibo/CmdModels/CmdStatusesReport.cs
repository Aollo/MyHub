using RestSharp;
using WeiboSDKForWinRT;

namespace MyHub.Models.Weibo
{
    public class CmdStatusesReport : ICustomCmdBase
    {
        private string _id = string.Empty;//要转发的微博ID。 必需参数
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _status = string.Empty;//添加的转发文本，必须做URLencode，内容不超过140个汉字，不填则默认为“转发微博”。 
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _is_comment = string.Empty;//是否在转发的同时发表评论，0：否、1：评论给当前微博、2：评论给原微博、3：都评论，默认为0 。 
        public string Is_comment
        {
            get { return _is_comment; }
            set { _is_comment = value; }
        }

        private string _rip = string.Empty;//开发者上报的操作用户真实IP，形如：211.156.0.1。
        public string Rip
        {
            get { return _rip; }
            set { _rip = value; }
        }

        public void ConvertToRequestParam(RestRequest request)
        {
            request.Resource = "/statuses/repost.json";
            request.Method = Method.POST;

            request.AddParameter("id", Id);

            if (Status.Length > 0)
            {
                request.AddParameter("status", Status);
            }
            if (Is_comment.Length > 0)
            {
                request.AddParameter("is_comment", Is_comment);
            }
            if (Rip.Length > 0)
            {
                request.AddParameter("Rip", Rip);
            }
        }
    }
}
