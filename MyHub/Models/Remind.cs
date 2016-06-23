namespace MyHub.Models
{
    public class Remind
    {
        public SnsType Sns { get; set; }

        public int Status { get; set; }

        public int Comment { get; set; }

        public int MentionStatus { get; set; }

        public int MentionComment { get; set; }

        public int Notice { get; set; }
    }
}
