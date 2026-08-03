using Listen2Me.MVVM.Initialization;
using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence.Syncing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Listen2Me.WPF.Modules;

public class HostedServices : IModule
{
    public string Name { get; } = "HostedServices";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IHostedService, PostgresSyncReconciliationService>();
        services.AddSingleton<IHostedService, StartupScanService>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    { }

    public void RegisterViews(IViewRegistry registry)
    { }
}