using System;
using System.Threading.Tasks;
using WeiboSDKForWinRT;
using MyHub.Lifecycle;

namespace MyHub.Services
{
    public class WeiboSnsAuthorization : IAuthorizationService
    {
        ClientOAuth weiboClientOAuth;

        public WeiboSnsAuthorization()
        {
            weiboClientOAuth = new ClientOAuth();

            // 编译运行之前需要开放平台参数
            SdkData.AppKey = Lifecycle.WeiboConstant.app_key;
            SdkData.AppSecret = Lifecycle.WeiboConstant.app_secret;
            SdkData.RedirectUri = Lifecycle.WeiboConstant.redirect_uri;
        }

        public async Task DoAuthorization()
        {
            if (true)// weiboClientOAuth.IsAuthorized
            {
                weiboClientOAuth.LoginCallback += async (isSucces, err, response) =>
                {
                    Models.Account account = AppRuntimeEnvironment.Instance.GetUserAccount("新浪微博");
                    if (account == null)
                        account = new Models.Account();
                    account.Sns = new Models.SnsType { Name = "新浪微博" };
                    account.isAvailable = false;// 默认授权信息不可用

                    if (isSucces)
                    {
                        // 保存授权信息到全局数据中心
                        account.AccessToken = response.AccessToken;
                        account.RefreshToken = response.RefreshToken;
                        account.ExpiresIn = DateTime.Now.AddSeconds(Convert.ToDouble(response.ExpriesIn));
                        account.UserId = response.Uid;
                        account.isAvailable = true;

                        var entity = await WeiboSnsDataAccessMethods.GetUserInfomation(account.UserId, "");
                        if(entity != null)
                        {
                            account.UserName = entity.screen_name;
                            account.LogoUrl = entity.profile_image_url;
                        }
                    }
                    else
                    {
                        // 授权信息不可用
                        account.isAvailable = false;
                    }

                    if (account.isAvailable)
                        AppRuntimeEnvironment.Instance.SetUserAccount(account);// 将更改保存到全局数据中心
                };
                weiboClientOAuth.BeginOAuth();
            }

            return;
        }
    }
}
