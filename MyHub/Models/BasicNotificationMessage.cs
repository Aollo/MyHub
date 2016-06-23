using System;

namespace MyHub.Models
{
    /// <summary>
    /// 基本的通知消息，可以适用于通知中心的提到、评论、点赞通知的消息实体
    /// </summary>
    public class BasicNotificationMessage
    {
        public SnsType Sns { get; set; }

        public NotificationMessageType MessageType { get; set; }

        public string MessageId { get; set; }

        public User Author { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public string Source { get; set; }

        public Status OriginalStatus { get; set; }
    }

    public enum NotificationMessageType
    {
        Mentions,
        Comments,
        Likes
    }
}
