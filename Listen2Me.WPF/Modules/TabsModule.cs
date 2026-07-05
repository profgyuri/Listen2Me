using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels.Tabs.Settings;
using Listen2Me.WPF.Views.Tabs.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Modules;

public class TabsModule : IModule
{
    public string Name { get; } = "Tabs";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<AppearanceTab>();
        services.AddSingleton<AppearanceTabViewModel>();
        
        services.AddSingleton<GeneralTab>();
        services.AddSingleton<GeneralTabViewModel>();

        services.AddSingleton<LibraryTab>();
        services.AddSingleton<LibraryTabViewModel>();

        services.AddSingleton<PlaybackTab>();
        services.AddSingleton<PlaybackTabViewModel>();
        
        services.AddSingleton<StorageTab>();
        services.AddSingleton<StorageTabViewModel>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
    }

    public void RegisterShells(IShellRegistry registry)
    {
    }
}