using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    [DataContract]
    public class WeiboPositionEntity
    {
        [DataMember]
        internal List<WeiboPositionItemEntity> pois;

        [DataMember]
        internal int total_number;
    }

    [DataContract]
    public class WeiboPositionItemEntity
    {
        [DataMember]
        internal string poiid;

        [DataMember]
        internal string title;

        [DataMember]
        internal string address;

        [DataMember]
        internal string lon;

        [DataMember]
        internal string lat;

        [DataMember]
        internal string category;

        [DataMember]
        internal string province;

        [DataMember]
        internal string city;
    }
}
