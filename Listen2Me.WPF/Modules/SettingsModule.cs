using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Settings.Appearance.Themes;
using Listen2Me.WPF.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Modules;

public class SettingsModule : IModule
{
    public string Name { get; } = "Settings";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IThemeManager, ThemeManager>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
    }

    public void RegisterShells(IShellRegistry registry)
    {
    }
}