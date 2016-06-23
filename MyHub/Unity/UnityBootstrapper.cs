using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace MyHub.Unity
{
    /// <summary>
    /// Helps initializing the Unity dependency container.
    /// </summary>
    public static class UnityBootstrapper
    {
        /// <summary>
        /// Gets or sets the dependency container.
        /// </summary>
        public static UnityContainer Container { get; set; }

        /// <summary>
        /// Gets the registries.
        /// </summary>
        public static List<IRegistry> Registries { get; private set; }

        private static void AddRegistries()
        {
            Registries.Add(new ServicesRegistry(Container));
        }

        /// <summary>
        /// Configures all registered dependencies.
        /// </summary>
        /// <remarks>
        /// The <see cref="ServiceLocator" /> needs to be initialized
        /// when calling this method.
        /// </remarks>
        public static void ConfigureRegistries()
        {
            AddRegistries();
            Registries.ForEach(r => r.Configure());
        }

        /// <summary>
        /// Initializes the <see cref="ServiceLocator" />.
        /// </summary>
        public static void Init()
        {
            Container = new UnityContainer();
            Registries = new List<IRegistry>();

            var serviceLocator = new UnityServiceLocator(Container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }
    }
}
