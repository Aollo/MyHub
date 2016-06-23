using Microsoft.Practices.Unity;

namespace MyHub.Unity
{
    /// <summary>
    /// Provides extensions for <see cref="IUnityContainer" />.
    /// </summary>
    public static class UnityExtensions
    {
        /// <summary>
        /// Registers a type mapping with the container.
        /// </summary>
        /// <typeparam name="TFrom">System.Type that will be requested.</typeparam>
        /// <typeparam name="TTo">System.Type that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        public static void RegisterTypeWithName<TFrom, TTo>(this IUnityContainer container) where TTo : TFrom
        {
            container.RegisterType<TFrom, TTo>(typeof(TTo).FullName);
        }
    }
}
