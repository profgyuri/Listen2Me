using Listen2Me.MVVM.Messages.Queuing;
using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Modules;

public class ServicesModule : IModule
{
    public string Name { get; } = "Services";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IMessageQueue, MessageQueue>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
        
    }

    public void RegisterViews(IViewRegistry registry)
    {
        
    }
}