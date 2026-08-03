using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Modules;

public class ComponentsModule : IModule
{
    public string Name { get; } = "Components";
    
    public void RegisterServices(IServiceCollection services)
    {
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
    }

    public void RegisterViews(IViewRegistry registry)
    {
    }
}