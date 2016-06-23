
namespace MyHub.Unity
{
    /// <summary>
    /// Defines a registry that takes care of registering
    /// multiple dependencies to a dependency container.
    /// </summary>
    public interface IRegistry
    {
        /// <summary>
        /// Configures dependencies.
        /// </summary>
        void Configure();
    }
}
