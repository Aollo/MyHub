namespace MyHub.Models
{
    public class UserProfile
    {
        public User BasicUserInfo { get; set; }

        public string LogoLargeUrl { get; set; }

        public string Gender { get; set; }

        public string Signature { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public int FollowerCount { get; set; }

        public int FriendsCount { get; set; }

        public int StatusesCount { get; set; }
    }
}
