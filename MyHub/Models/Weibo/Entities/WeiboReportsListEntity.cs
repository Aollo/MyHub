using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// http://open.weibo.com/wiki/2/statuses/repost_timeline
    /// </summary>
    [DataContract]
    public class WeiboReportsListEntity
    {
        [DataMember]
        private List<WeiboStatusEntity> reposts;
        public List<WeiboStatusEntity> Reposts
        {
            get
            {
                return this.reposts;
            }
            set
            {
                this.reposts = value;
            }
        }

        [DataMember]
        private Int64 previous_cursor;
        public Int64 Previous_cursor
        {
            get
            {
                return this.previous_cursor;
            }
            set
            {
                this.previous_cursor = value;
            }
        }

        [DataMember]
        private Int64 next_cursor;
        public Int64 Next_cursor
        {
            get
            {
                return this.next_cursor;
            }
            set
            {
                this.next_cursor = value;
            }
        }

        [DataMember]
        private UInt64 total_number;
        public UInt64 Total_number
        {
            get
            {
                return this.total_number;
            }
            set
            {
                this.total_number = value;
            }
        }
    }
}
