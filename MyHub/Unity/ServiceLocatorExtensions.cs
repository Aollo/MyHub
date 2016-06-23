using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace MyHub.Unity
{
    /// <summary>
    /// Provides extensions for <see cref="IServiceLocator" />.
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        /// <summary>
        /// Gets an instance of the given TService.
        /// </summary>
        /// <typeparam name="TService">The requested type.</typeparam>
        /// <param name="serviceLocator">The service locator instance.</param>
        /// <param name="createNew">
        /// If true, a new instance will be returned and the existing one in the service locator
        /// overwritten.
        /// </param>
        /// <returns>The instance of TService.</returns>
        public static TService GetInstance<TService>(this IServiceLocator serviceLocator, bool createNew)
        {
            if (createNew)
            {
                UnityBootstrapper.Container.RegisterType(typeof(TService), typeof(TService),
                    new ContainerControlledLifetimeManager());
            }

            return serviceLocator.GetInstance<TService>();
        }
    }
}
