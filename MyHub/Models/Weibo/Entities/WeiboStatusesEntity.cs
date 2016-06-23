using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyHub.Models.Weibo
{
    /// <summary>
    /// 新鲜事的列表的类，对应的API调用返回的微博（status)数据结构
    /// </summary>
    [DataContract]
    public class WeiboStatusesEntity
    {
        public WeiboStatusesEntity()
        {
        }

        [DataMember]
        private List<WeiboStatusEntity> statuses;
        public List<WeiboStatusEntity> Statuses
        {
            get
            {
                return this.statuses;
            }

            set
            {
                this.statuses = value;
            }
        }

        [DataMember]
        private Boolean hasvisible;
        public Boolean Hasvisible
        {
            get { return this.hasvisible; }
            set { this.hasvisible = value; }
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
        private Int64 total_number;
        public Int64 Total_number
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


    /// <summary>
    /// 微博新鲜事实体类
    /// </summary>
    [DataContract]
    public class WeiboStatusEntity
    {
        [DataMember]
        internal string created_at;

        [DataMember]
        internal Int64 id;

        [DataMember]
        internal string mid;

        [DataMember]
        internal string idstr;

        [DataMember]
        internal string text;

        [DataMember]
        internal string source;

        [DataMember]
        internal Boolean favorited;

        [DataMember]
        internal Boolean truncated;

        [DataMember]
        internal string in_reply_to_status_id;

        [DataMember]
        internal string in_reply_to_user_id;

        [DataMember]
        internal string in_reply_to_screen_name;

        [DataMember]
        internal List<Picture_urls> pic_urls;

        [DataMember]
        internal string thumbnail_pic;

        [DataMember]
        internal string bmiddle_pic;

        [DataMember]
        internal string original_pic;

        [DataMember]
        internal GeoEntity geo;

        [DataMember]
        internal WeiboUserEntity user;

        [DataMember]
        internal WeiboStatusEntity retweeted_status;

        [DataMember]
        internal int reposts_count;

        [DataMember]
        internal int comments_count;

        [DataMember]
        internal int attitudes_count;

        [DataMember]
        internal int mlevel;

        [DataMember]
        internal VisibleEntity visible;
    }

    /// <summary>
    /// 新鲜事中的pic_urls对应的类
    /// </summary>
    [DataContract]
    public class Picture_urls
    {
        [DataMember]
        internal string thumbnail_pic;
    }

    /// <summary>
    /// 微博中的Visible属性对应的类
    /// </summary>
    [DataContract]
    public class VisibleEntity
    {
        [DataMember]
        internal int type;

        [DataMember]
        internal int list_id;
    }
}
