using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    [DataContract]
    public class WeiboFavoriteStatusesListEntity
    {
        [DataMember]
        internal List<WeiboFavoriteStatusEntity> favorites;

        [DataMember]
        internal int total_number;
    }

    [DataContract]
    public class WeiboFavoriteStatusEntity
    {
        [DataMember]
        internal WeiboStatusEntity status;

        [DataMember]
        internal List<WeiboFavoriteStatusTagEntity> tags;

        [DataMember]
        internal string favorited_time;
    }

    [DataContract]
    public class WeiboFavoriteStatusTagEntity
    {
        [DataMember]
        internal int id;

        [DataMember]
        internal string tag;
    }
}
