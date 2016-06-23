using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using MyHub.Services;

namespace MyHub.Unity
{
    /// <summary>
    /// Registry for services.
    /// </summary>
    public class ServicesRegistry : RegistryBase, IRegistry
    {
        private string _weiboName = "新浪微博";
        private string _kaixinName = "开心网";

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="container">The Unity container.</param>
        public ServicesRegistry(IUnityContainer container) : base(container)
        {
        }

        /// <summary>
        /// Configures dependencies.
        /// </summary>
        public void Configure()
        {
            Container.RegisterType<IAuthorizationService, WeiboSnsAuthorization>(_weiboName);
            Container.RegisterType<IAuthorizationService, KaixinSnsAuthorization>(_kaixinName);
            Container.RegisterType<ISnsDataService, WeiboSnsDataService>(_weiboName, new ContainerControlledLifetimeManager());
            Container.RegisterType<ISnsDataService, KaixinSnsDataService>(_kaixinName, new ContainerControlledLifetimeManager());
            Container.RegisterType<ILocalDataService, LocalDataService>();
        }
    }
}
