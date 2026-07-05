using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Modules;

public class DataModule : IModule
{
    public string Name { get; } = "Data";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<SqLiteDataContext>();
        services.AddTransient<PostgresContextFactory>();
        services.AddTransient<ISharedDbContext, SharedDataContext>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
    }

    public void RegisterShells(IShellRegistry registry)
    {
    }
}