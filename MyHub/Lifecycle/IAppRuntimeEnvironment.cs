
namespace MyHub.Lifecycle
{
    public interface IAppRuntimeEnvironment
    {
        /// <summary>
        /// 获取登录的用户账户信息
        /// 当前只支持每个社交网络登录一个账号，如果需要支持多账号需要修改此处
        /// </summary>
        /// <param name="snsName">社交账号类型名字，需要与Account.SnsType.SnsName相对应</param>
        /// <returns></returns>
        Models.Account GetUserAccount(string snsName);

        Models.Account[] GetAllUserAccount();

        /// <summary>
        /// 设置登录的用户账户信息，通过社交网络的名字进行索引
        /// 当前只支持每个社交网络登录一个账号，如果需要支持多账号需要修改此处
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        bool SetUserAccount(Models.Account account);

        void SetFrame(MyHubEnums.NavigationFrameType type, Windows.UI.Xaml.Controls.Frame frame);

        Windows.UI.Xaml.Controls.Frame GetFrame(MyHubEnums.NavigationFrameType type);
    }
}
