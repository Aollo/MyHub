
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 消息未读数（remind）字段
    /// </summary>
    [DataContract]
    public class WeiboRemindEntity
    {
        [DataMember]
        internal int status;//新微博未读数 

        [DataMember]
        internal int follower;//新粉丝数 

        [DataMember]
        internal int cmt;//新评论数 

        [DataMember]
        internal int dm;//新私信数 

        [DataMember]
        internal int mention_status;//新提及我的微博数 

        [DataMember]
        internal int mention_cmt;//新提及我的评论数 

        [DataMember]
        internal int group;//微群消息未读数 

        [DataMember]
        internal int private_group;//私有微群消息未读数 

        [DataMember]
        internal int notice;//新通知未读数 

        [DataMember]
        internal int invite;//新邀请未读数 

        [DataMember]
        internal int badge;//新勋章数 

        [DataMember]
        internal int photo;//相册消息未读数 

        [DataMember]
        internal int msgbox;//{{{3}}} 
    }
}
