namespace MyHub.Models.LocalDB
{
    public class AccountTableEntity
    {
        public long account_id;

        public long sns_id;

        public string access_token;

        public string refresh_token;

        /// <summary>
        /// 表示授权码过期的日期和时间，注意是string类型，用到的时候需要转化成DateTime
        /// </summary>
        public string expires_in;

        public string user_id;

        public string user_name;

        public string user_logourl;

        /// <summary>
        /// 表示账户当前是否有效，注意是long类型，用到的时候需要将其转化为bool类型
        /// </summary>
        public long is_available;
    }
}
