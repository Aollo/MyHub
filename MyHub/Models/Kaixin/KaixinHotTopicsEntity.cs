using System.Runtime.Serialization;

namespace MyHub.Models.Kaixin
{
    [DataContract]
    public class KaixinHotTopicEntity
    {
        [DataMember]
        internal string title;

        [DataMember]
        internal string word;

        [DataMember]
        internal string num;
    }
}
