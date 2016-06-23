
using System.Runtime.Serialization;

namespace MyHub.Models.Kaixin
{
    [DataContract]
    public class KaixinMessageSummaryEntity
    {
        [DataMember]
        internal string message;

        [DataMember]
        internal string sysmsg_notice;

        [DataMember]
        internal string sysmsg_friends;

        [DataMember]
        internal string sysmsg_birthday;

        [DataMember]
        internal string sysmsg_mention;

        [DataMember]
        internal string sysmsg_forward;

        [DataMember]
        internal string bbs_msg;

        [DataMember]
        internal string bbs_reply;

        [DataMember]
        internal string rgroup_msg;

        [DataMember]
        internal string rgroup_reply;

        [DataMember]
        internal string comment;

        [DataMember]
        internal string reply;
    }
}
