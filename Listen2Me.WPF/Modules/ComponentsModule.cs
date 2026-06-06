using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels.Components;
using Listen2Me.WPF.Views.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Modules;

public class ComponentsModule : IModule
{
    public string Name { get; } = "Components";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<NavBar>();
        services.AddSingleton<NavBarViewModel>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
    }

    public void RegisterShells(IShellRegistry registry)
    {
    }
}