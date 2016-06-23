
using System;
using System.Collections.Generic;
using MyHub.Models;

namespace MyHub.Services
{
    public class LocalDataService : ILocalDataService
    {
        // 保证数据库中每个类型的社交账号最多只有一个
        public string StoreAccount(Account account)
        {
            // 查社交网络类型ID
            var snsType = LocalDataAccessMethods.Query_SnsType("sns_name", account.Sns.Name);
            if (snsType == null)// 如果没有社交网络信息，插入
            {
                LocalDataAccessMethods.Insert_SnsType(account.Sns.Name);
                // 验证是否存入成功并获取sns_id
                snsType = LocalDataAccessMethods.Query_SnsType("sns_name", account.Sns.Name);
                if (snsType == null)// 出错
                    return null;
            }

            // 判断是否有该社交网络类型的社交账号
            if (LocalDataAccessMethods.Query_Account("sns_id", snsType.sns_id.ToString()) == null)
            {
                LocalDataAccessMethods.Insert_Account(snsType.sns_id.ToString(), account.AccessToken, account.RefreshToken, account.ExpiresIn.ToString(), account.UserId, account.UserName, account.LogoUrl, account.isAvailable ? "1" : "0");
                var entity = LocalDataAccessMethods.Query_Account("access_token", account.AccessToken);// access token 是一定唯一的
                if (entity == null)
                    return null;
                else
                    return entity.account_id.ToString();
            }
            else// 如果有该社交网络类型的社交账号，则根据sns_id更新数据库
            {
                var result = LocalDataAccessMethods.Update_Account(new Models.LocalDB.AccountTableEntity
                {
                    sns_id = snsType.sns_id,// sns_id是更新数据库的依据
                    account_id = account.LocalAccountId,// LocalAccountId不是更新数据库的依据，不是主键，可有可无
                    access_token = account.AccessToken,
                    refresh_token = account.RefreshToken,
                    user_id = account.UserId,
                    user_name = account.UserName,
                    user_logourl = account.LogoUrl,
                    expires_in = account.ExpiresIn.ToString(),
                    is_available = account.isAvailable ? 1 : 0
                });

                if (result)// 如果更新成功
                    return account.LocalAccountId.ToString();
                else
                    return null;
            }
            
        }

        public Account LoadAccount(string snsTypeName)
        {
            var snsEntity = LocalDataAccessMethods.Query_SnsType("sns_name", snsTypeName);
            if (snsEntity == null || snsEntity.sns_id.ToString() == null)
                return null;

            var accountEntity = LocalDataAccessMethods.Query_Account("sns_id", snsEntity.sns_id.ToString());
            if (accountEntity == null)
                return null;

            return new Account
            {
                Sns = new SnsType
                {
                    ID = (int)snsEntity.sns_id,
                    Name = snsEntity.sns_name
                },
                LocalAccountId = (int)accountEntity.account_id,
                AccessToken = accountEntity.access_token,
                RefreshToken = accountEntity.refresh_token,
                ExpiresIn = System.DateTime.Parse(accountEntity.expires_in),
                UserId = accountEntity.user_id,
                UserName = accountEntity.user_name,
                LogoUrl = accountEntity.user_logourl,
                isAvailable = accountEntity.is_available == 0 ? false : true
            };
        }

        public List<Account> LoadAllAccounts()
        {
            List<Account> accounts = new List<Account>();
            Account temp;

            temp = LoadAccount("新浪微博");
            if (temp != null)
                accounts.Add(temp);

            temp = LoadAccount("开心网");
            if (temp != null)
                accounts.Add(temp);

            return accounts;
        }

        public List<Account> LoadAllAvailableAccounts()
        {
            List<Account> accounts = new List<Account>();
            Account temp;

            temp = LoadAccount("新浪微博");
            if (temp != null && temp.isAvailable && temp.ExpiresIn > System.DateTime.Now)
                accounts.Add(temp);

            temp = LoadAccount("开心网");
            if (temp != null && temp.isAvailable && temp.ExpiresIn > System.DateTime.Now)
                accounts.Add(temp);

            return accounts;
        }

        public bool StoreSetting(UserSetting setting)
        {
            // 没有找到对应的设置项信息
            var settingEntity = LocalDataAccessMethods.Query_Setting("setting_name", setting.SettingName);
            if (settingEntity == null)
            {
                return LocalDataAccessMethods.Insert_Setting(setting.SettingName, setting.SettingValue);
            }
            else// 更新现有设置项
            {
                return LocalDataAccessMethods.Update_Setting(new Models.LocalDB.UserSettingTableEntity
                {
                    setting_id = settingEntity.setting_id,
                    setting_name = setting.SettingName,
                    setting_value = setting.SettingValue
                });
            }
        }

        public UserSetting LoadSetting(string settingName)
        {
            var settingEntity = LocalDataAccessMethods.Query_Setting("setting_name", settingName);

            if (settingEntity == null)
                return null;

            return new UserSetting
            {
                SettingId = (int)settingEntity.setting_id,
                SettingName = settingEntity.setting_name,
                SettingValue = settingEntity.setting_value
            };
        }

        public async System.Threading.Tasks.Task<Location> GetCurrentLocation()
        {
            Location location = new Location();

            var geolocator = new Windows.Devices.Geolocation.Geolocator();
            var geoposition = await geolocator.GetGeopositionAsync();
            var geocoordinate = geoposition.Coordinate;

            location.Latitude = geocoordinate.Point.Position.Latitude.ToString("0.00");
            location.Longitude = geocoordinate.Point.Position.Longitude.ToString("0.00");

            if(geoposition.CivicAddress != null)
            {
                location.Country = geoposition.CivicAddress.Country;
                location.Province = geoposition.CivicAddress.State;
                location.City = geoposition.CivicAddress.City;
            }

            return location;
        }
    }
}
