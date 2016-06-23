
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 微博地理信息字段
    /// </summary>
    [DataContract]
    public class GeoEntity
    {
        [DataMember]
        internal string longitude;

        [DataMember]
        internal string latitude;

        [DataMember]
        internal string city;

        [DataMember]
        internal string province;

        [DataMember]
        internal string city_name;

        [DataMember]
        internal string province_name;

        [DataMember]
        internal string address;

        [DataMember]
        internal string pinyin;

        [DataMember]
        internal string more;
    }
}
