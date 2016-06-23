using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    [DataContract]
    public class WeiboUsersListEntity
    {
        [DataMember]
        internal List<WeiboUserEntity> users;

        [DataMember]
        internal int total_number;

        [DataMember]
        internal int display_total_number;

        [DataMember]
        internal int next_cursor;

        [DataMember]
        internal int previous_cursor;
    }

    /// <summary>
    /// 微博用户信息类
    /// </summary>
    [DataContract]
    public class WeiboUserEntity
    {
        [DataMember]
        internal Int64 id;

        [DataMember]
        internal string idstr;

        [DataMember]
        internal string screen_name;

        [DataMember]
        internal string name;

        [DataMember]
        internal string province;

        [DataMember]
        internal string city;

        [DataMember]
        internal string location;

        [DataMember]
        internal string description;

        [DataMember]
        internal string url;

        [DataMember]
        internal string profile_image_url;

        [DataMember]
        internal string cover_image;

        [DataMember]
        internal string profile_url;

        [DataMember]
        internal string domain;

        [DataMember]
        internal string weihao;

        [DataMember]
        internal string gender;

        [DataMember]
        internal int followers_count;

        [DataMember]
        internal int friends_count;

        [DataMember]
        internal int statuses_count;

        [DataMember]
        internal int favourites_count;

        [DataMember]
        internal string created_at;

        [DataMember]
        internal Boolean following;

        [DataMember]
        internal Boolean allow_all_act_msg;

        [DataMember]
        internal Boolean geo_enabled;

        [DataMember]
        internal Boolean verified;

        [DataMember]
        internal int verified_type;

        [DataMember]
        internal string remark;

        [DataMember]
        internal WeiboStatusEntity status;

        [DataMember]
        internal Boolean allow_all_comment;

        [DataMember]
        internal string avatar_large;

        [DataMember]
        internal string avatar_hd;

        [DataMember]
        internal string verified_reason;

        [DataMember]
        internal Boolean follow_me;

        [DataMember]
        internal int online_status;

        [DataMember]
        internal int bi_followers_count;

        [DataMember]
        internal string lang;
    }
}
