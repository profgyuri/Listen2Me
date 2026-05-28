using Microsoft.Extensions.DependencyInjection;
using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels;

namespace Listen2Me.WPF.Modules;

/// <summary>
/// Registers the default shell module.
/// </summary>
public sealed class ShellModule : IModule
{
    /// <inheritdoc />
    public string Name => "Shell";

    /// <inheritdoc />
    public void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<DefaultHomeViewModel>();
    }

    /// <inheritdoc />
    public void RegisterNavigation(INavigationRegistry registry)
    {
        registry.Register<DefaultHomeViewModel>("home");
    }
}