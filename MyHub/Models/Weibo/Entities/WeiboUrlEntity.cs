
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 将一个或多个长链接转换成短链接 
    /// API文档：http://open.weibo.com/wiki/2/short_url/shorten
    /// </summary>
    [DataContract]
    public class WeiboUrlEntity
    {
        [DataContract]
        public class UrlEntity
        {
            [DataMember]
            internal string url_short;

            [DataMember]
            internal string url_long;

            [DataMember]
            internal int type;

            [DataMember]
            internal bool result;
        }

        [DataMember]
        internal List<UrlEntity> urls;
    }
}
