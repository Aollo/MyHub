using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using KaixinSDK;
using KaixinSDK.Entities;
using MyHub.Lifecycle;

namespace MyHub.Services
{
    public class KaixinSnsAuthorization : IAuthorizationService
    {
        OAuthEntity kaixinClientOAuth;

        public KaixinSnsAuthorization()
        {
            kaixinClientOAuth = new OAuthEntity();
        }

        public async Task DoAuthorization()
        {
            Models.Account account = AppRuntimeEnvironment.Instance.GetUserAccount("开心网");
            if (account == null)
                account = new Models.Account();
            account.Sns = new Models.SnsType { Name = "开心网" };

            try
            {
                string code = await KaixinGetAuthorizeCode();

                kaixinClientOAuth = await API.Instance.Login(KaixinConstant.consumer_key, KaixinConstant.consumer_secret, code, KaixinConstant.redirect_uri);

                if (kaixinClientOAuth != null && kaixinClientOAuth.Access_Token != null)
                {
                    var entity = await KaixinSnsDataAccessMethods.GetUserInfo(kaixinClientOAuth.Access_Token, "");

                    // 保存授权信息
                    account.AccessToken = kaixinClientOAuth.Access_Token;
                    account.RefreshToken = kaixinClientOAuth.Refresh_Token;
                    account.ExpiresIn = DateTime.Now.AddSeconds(Convert.ToDouble(kaixinClientOAuth.Expires_In));
                    account.isAvailable = true;
                    if(entity != null)
                    {
                        account.UserId = entity.Uid.ToString();
                        account.UserName = entity.Name;
                        account.LogoUrl = entity.Logo120 != null ? entity.Logo120 : entity.Logo50;
                    }
                }
                else
                {
                    // 授权信息不可用
                    account.isAvailable = false;
                }

            }
            catch
            {
                // 授权信息不可用
                account.isAvailable = false;
            }
            finally
            {
                if (account.isAvailable)
                    AppRuntimeEnvironment.Instance.SetUserAccount(account);// 将更改保存到全局数据中心
            }

            return;
        }

        /// <summary>
        /// 开心网  获取 Authorization Code。
        /// </summary>
        /// <returns></returns>
        private async Task<string> KaixinGetAuthorizeCode()
        {
            // 生成授权页面的 Url
            var uriString = API.Instance.GenerateAuthorizeUrl(KaixinConstant.consumer_key, KaixinConstant.redirect_uri, string.Join(" ", KaixinConstant.Scope));

            // 获取 Authorization Code
            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(uriString), new Uri(KaixinConstant.redirect_uri));
            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var query = result.ResponseData.ToString().Split(new char[] { '?', '&' });
                var code = query.Where(x => x.Length > 5 && string.Compare(x.Substring(0, 5), "code=", StringComparison.OrdinalIgnoreCase) == 0).First();
                return code.Substring(5);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
