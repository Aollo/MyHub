using System;

namespace MyHub.Models
{
    public class User
    {
        public SnsType Sns { get; set; }

        public string UserId { get; set; }

        public string NickName { get; set; }

        public string RemarkName { get; set; }

        public string LogoUrl { get; set; }

        public bool Verified { get; set; }

        public bool Following { get; set; }
    }
}
