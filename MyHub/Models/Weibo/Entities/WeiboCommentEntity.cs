using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    [DataContract]
    public class WeiboCommentEntity
    {
        [DataMember]
        internal string created_at;//评论创建时间 

        [DataMember]
        internal Int64 id;//评论的ID

        [DataMember]
        internal string text;//评论的内容 

        [DataMember]
        internal string source;//评论的来源 

        [DataMember]
        internal WeiboUserEntity user;//评论作者的用户信息字段 

        [DataMember]
        internal string mid;//评论的MID 

        [DataMember]
        internal string idstr;//字符串型的评论ID 

        [DataMember]
        internal WeiboStatusEntity status;//评论的微博信息字段

        [DataMember]
        internal WeiboCommentEntity reply_comment;//评论来源评论，当本评论属于对另一评论的回复时返回此字段 
    }
    
    [DataContract]
    public class WeiboCommentsListEntity
    {
        [DataMember]
        internal List<WeiboCommentEntity> comments;

        [DataMember]
        internal Int64 previous_cursor;

        [DataMember]
        internal Int64 next_cursor;

        [DataMember]
        internal Int64 total_number;
    }
}
