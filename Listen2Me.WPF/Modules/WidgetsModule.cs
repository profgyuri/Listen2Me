using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels.Widgets;
using Listen2Me.WPF.Views.Widgets;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Modules;

public class WidgetsModule : IModule
{
    public string Name { get; } = "Widgets";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<PlayerControlsWidget>();
        services.AddSingleton<PlayerControlsWidgetViewModel>();
        
        services.AddSingleton<PlaylistWidget>();
        services.AddSingleton<PlaylistWidgetViewModel>();
        
        services.AddSingleton<SearchResultsWidget>();
        services.AddSingleton<SearchResultsWidgetViewModel>();
        
        services.AddSingleton<TrackInfoWidget>();
        services.AddSingleton<TrackInfoWidgetViewModel>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    { }

    public void RegisterShells(IShellRegistry registry)
    { }
}