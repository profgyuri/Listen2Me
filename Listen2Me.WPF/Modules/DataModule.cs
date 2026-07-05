using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence;
using Listen2Me.MVVM.Persistence.Syncing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Listen2Me.WPF.Modules;

public class DataModule : IModule
{
    public string Name { get; } = "Data";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<SqLiteDataContext>();
        services.AddScoped<PostgresContextFactory>();
        services.AddScoped<ISharedDbContext, SharedDataContext>();
        services.AddScoped<ISyncService, SyncService>();

        services.AddSingleton<IHostedService, PostgresSyncReconciliationService>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
    }

    public void RegisterShells(IShellRegistry registry)
    {
    }
}