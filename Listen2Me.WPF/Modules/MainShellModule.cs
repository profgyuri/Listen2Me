using Microsoft.Extensions.DependencyInjection;
using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels.Layouts;
using Listen2Me.MVVM.ViewModels.Shells;
using Listen2Me.WPF.Views.Layouts;
using Listen2Me.WPF.Views.Shells;

namespace Listen2Me.WPF.Modules;

/// <summary>
/// Registers the default shell module.
/// </summary>
public sealed class MainShellModule : IModule
{
    public string Name { get; } = "MainShell";

    /// <inheritdoc />
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<MainShell>();
        services.AddSingleton<MainShellViewModel>();
        services.AddSingleton<DefaultMainLayout>();
        services.AddSingleton<MainLayoutViewModel>();
    }

    /// <inheritdoc />
    public void RegisterNavigation(INavigationRegistry registry)
    {
        registry.Register<MainLayoutViewModel>("home");
    }
}