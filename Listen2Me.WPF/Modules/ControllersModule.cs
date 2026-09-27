using Listen2Me.MVVM.Controllers;
using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Modules;

public class ControllersModule : IModule
{
    public string Name { get; } = "Controllers";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IDragNDropController, DragNDropController>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    { }

    public void RegisterViews(IViewRegistry registry)
    { }
}