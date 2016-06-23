using System;

namespace MyHub.Models
{
    public class Comment
    {
        public SnsType Sns { get; set; }

        public string CommentId { get; set; }

        public DateTime CreateTime { get; set; }

        public string Content { get; set; }
        
        public string Source { get; set; }

        public User Author { get; set; }

        public Status StatusInfo { get; set; }

        public Comment ReplyComment { get; set; }
    }
}
