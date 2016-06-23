using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHub.Lifecycle
{
    /// <summary>
    /// 新浪微博常量数据
    /// </summary>
    public static class WeiboConstant
    {
        internal static readonly string app_key = "";

        internal static readonly string app_secret = "";

        internal static readonly string redirect_uri = "https://api.weibo.com/oauth2/default.html";
    }


    /// <summary>
    /// 开心网的常量数据
    /// </summary>
    public static class KaixinConstant
    {
        /// <summary>
        /// 申请组件时获得的 API Key。
        /// </summary>
        internal static readonly string consumer_key = "";

        /// <summary>
        /// 应用的 Secret Key。
        /// </summary>
        internal static readonly string consumer_secret = "";

        /// <summary>
        /// 授权后要回调的 URI，即接收 Authorization Code 的 URI。
        /// </summary>
        internal static readonly string redirect_uri = "http://open.kaixin001.com";

        /// <summary>
        /// 以空格分隔的权限列表，默认为 basic 权限。
        /// </summary>
        internal static readonly string[] Scope =
        {
            "basic",
            "user_intro",
            "user_birthday",
            "create_records",
            "friends_records",
            "user_comment",
        };

        internal struct METHODS
        {
            internal static readonly string DeleteRecord = "https://api.kaixin001.com/records/delete.json";
            internal static readonly string LikeCheck = "https://api.kaixin001.com/like/check.json";
            internal static readonly string RecordsUser = "https://api.kaixin001.com/records/user.json";
            internal static readonly string RecordsPublic = "https://api.kaixin001.com/records/public.json";
            internal static readonly string RecordsTopics = "https://api.kaixin001.com/records/topics.json";
            internal static readonly string MessageSummary = "https://api.kaixin001.com/msg/summary.json";
            internal static readonly string MessageClear = "https://api.kaixin001.com/msg/clear.json";
        };
    }

    /// <summary>
    /// 记录了所有的枚举类型
    /// </summary>
    public static class MyHubEnums
    {
        public enum NavigationFrameType
        {
            LeftPart,
            RightPart,
            MainFrame
        };

        public enum StatusInteractionType
        {
            Repost,
            Comment,
            Like,
            Favorite
        };

        public enum NotificationMessageType
        {
            Mentions,
            Comments,
            Likes
        };

        public enum ExploreType
        {
            PublicStatus,
            LocalStatus,
            HotTopics
        };

        public enum UserProfilePivotSelectionType
        {
            Home,
            Statuses,
            Photos
        };

        public enum NavigatedToPostStatusPageType
        {
            PostNewStatus,
            Repost,
            Comment,
            ReplyComment,
            RepostComment,
            TransferHotTopic,
            TransferMentionUser,
            TransferLocation,
        };
    }
}
