
using Microsoft.Practices.Unity;

namespace MyHub.Unity
{
    /// <summary>
    /// Base class for registering implementations to the Unity container.
    /// </summary>
    public class RegistryBase
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="container">The Unity container.</param>
        protected RegistryBase(IUnityContainer container)
        {
            Container = container;
        }

        /// <summary>
        /// The Unity container.
        /// </summary>
        protected IUnityContainer Container { get; set; }
    }
}
