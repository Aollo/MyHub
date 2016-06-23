
namespace MyHub.Models
{
    public class Account
    {
        /// <summary>
        /// the sns type of this account
        /// </summary>
        public SnsType Sns { get; set; }

        public int LocalAccountId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public System.DateTime ExpiresIn { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string LogoUrl { get; set; }

        public bool isAvailable { get; set; }
    }
}
