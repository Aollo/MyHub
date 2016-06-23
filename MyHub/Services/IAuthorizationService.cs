using System.Threading.Tasks;

namespace MyHub.Services
{
    public interface IAuthorizationService
    {
        /// <summary>
        /// 执行应用程序授权
        /// </summary>
        /// <returns>授权之后的账户授权信息，信息可能不完全</returns>
        Task DoAuthorization();
    }
}
