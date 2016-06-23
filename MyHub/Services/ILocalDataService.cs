using System.Collections.Generic;

namespace MyHub.Services
{
    /// <summary>
    /// 【没有进行完全测试】
    /// </summary>
    public interface ILocalDataService
    {
        /// <summary>
        /// 将用户账户信息存入数据库，目前只能支持每个社交网络一个社交账号，
        /// 如果后续需要更改多账户支持，需要修改此处
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        string StoreAccount(Models.Account account);

        /// <summary>
        /// 根据社交网络名称载入社交账号信息
        /// </summary>
        /// <param name="snsTypeName">新浪微博、开心网 其中之一</param>
        /// <returns></returns>
        Models.Account LoadAccount(string snsTypeName);

        /// <summary>
        /// 载入所有账户信息，不管是否可用
        /// </summary>
        /// <returns></returns>
        List<Models.Account> LoadAllAccounts();

        /// <summary>
        /// 载入所有可用的社交网络账号信息，
        /// 如果后续需要更改多账户支持，需要修改此处
        /// </summary>
        /// <returns></returns>
        List<Models.Account> LoadAllAvailableAccounts();

        /// <summary>
        /// 保存用户设置到数据库
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        bool StoreSetting(Models.UserSetting setting);

        /// <summary>
        /// 根据设置项名称载入用户设置，如果数据库支持原因，目前只支持单条载入设置
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        Models.UserSetting LoadSetting(string settingName);

        System.Threading.Tasks.Task<Models.Location> GetCurrentLocation();
    }
}
