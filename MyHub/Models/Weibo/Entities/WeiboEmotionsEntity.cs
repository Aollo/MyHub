using System;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class WeiboEmotionsEntity
    {
        [DataMember]
        internal string category;

        [DataMember]
        internal Boolean common;

        [DataMember]
        internal Boolean hot;

        [DataMember]
        internal string icon;

        [DataMember]
        internal string phrase;

        [DataMember]
        internal object picid;

        [DataMember]
        internal string type;

        [DataMember]
        internal string url;

        [DataMember]
        internal string value;
    }
}
