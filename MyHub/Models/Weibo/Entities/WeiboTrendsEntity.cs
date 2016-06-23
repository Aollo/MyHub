using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 解析 返回热门话题 返回Json的类
    /// http://open.weibo.com/wiki/2/trends/hourly
    /// http://open.weibo.com/wiki/2/trends/daily
    /// http://open.weibo.com/wiki/2/trends/weekly
    /// </summary>
    [DataContract]
    public class WeiboTrendsEntity
    {
        [DataMember]
        internal WeiboTrendTimeEntity trends;

        [DataMember]
        internal Int64 as_of;
    }

    [DataContract]
    public class WeiboTrendTimeEntity
    {
        [DataMember]
        List<WeiboTrendEntity> trendTime;
    }

    [DataContract]
    public class WeiboTrendEntity
    {
        [DataMember]
        internal string name;

        [DataMember]
        internal string query;

        [DataMember]
        internal string amount;

        [DataMember]
        internal string delta;
    }

}
