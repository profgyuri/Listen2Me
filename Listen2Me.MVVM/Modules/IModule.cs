using Microsoft.Extensions.DependencyInjection;
using Listen2Me.MVVM.Navigation;

namespace Listen2Me.MVVM.Modules;

/// <summary>
/// Defines a composable MVVM module.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Gets the unique module name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Registers module services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    void RegisterServices(IServiceCollection services);

    /// <summary>
    /// Registers module navigation routes.
    /// </summary>
    /// <param name="registry">The navigation registry.</param>
    void RegisterNavigation(INavigationRegistry registry);
}