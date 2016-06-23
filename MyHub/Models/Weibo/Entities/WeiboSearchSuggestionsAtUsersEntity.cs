using System;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// @用户时的联想建议 
    /// API文档：http://open.weibo.com/wiki/2/search/suggestions/at_users
    /// </summary>
    [DataContract]
    public class WeiboSearchSuggestionsAtUsersEntity
    {
        [DataMember]
        internal Int64 uid;

        [DataMember]
        internal string nickname;

        [DataMember]
        internal string remark;
    }

    [DataContract]
    public class WeiboSearchSuggestionUserEntity
    {
        [DataMember]
        internal string screen_name;

        [DataMember]
        internal string followers_count;

        [DataMember]
        internal string uid;
    }
}
