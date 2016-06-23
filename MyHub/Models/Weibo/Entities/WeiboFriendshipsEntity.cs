using System;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 解析 获取两个用户之间的详细关注关系情况 返回结果Json的类
    /// API文档：http://open.weibo.com/wiki/2/friendships/show
    /// </summary>
    [DataContract]
    public class WeiboFriendshipsEntity
    {
        [DataContract]
        public class FriendshipTargetUserEntity
        {
            [DataMember]
            internal Boolean followed_by;

            [DataMember]
            internal Boolean following;

            [DataMember]
            internal Int64 id;

            [DataMember]
            internal Boolean notifications_enabled;

            [DataMember]
            internal string screen_name;
        }

        [DataContract]
        public class FriendshipSourceUserEntity
        {
            [DataMember]
            internal Boolean followed_by;

            [DataMember]
            internal Boolean following;

            [DataMember]
            internal Int64 id;

            [DataMember]
            internal Boolean notifications_enabled;

            [DataMember]
            internal string screen_name;
        }

        [DataMember]
        internal FriendshipSourceUserEntity source;

        [DataMember]
        internal FriendshipTargetUserEntity target;
    }
}
