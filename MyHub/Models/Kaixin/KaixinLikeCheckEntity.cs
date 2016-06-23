using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyHub.Models.Kaixin
{
    [DataContract]
    public class KaixinLikeCheckListEntity
    {
        [DataMember]
        internal List<KaixinLikeCheckEntity> data;
    }

    [DataContract]
    public class KaixinLikeCheckEntity
    {
        [DataMember]
        internal string uid;

        [DataMember]
        internal string liked;
    }
}
