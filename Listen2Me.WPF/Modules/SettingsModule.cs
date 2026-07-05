using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence;
using Listen2Me.MVVM.Settings;
using Listen2Me.MVVM.Settings.Appearance;
using Listen2Me.MVVM.Settings.Appearance.Themes;
using Listen2Me.MVVM.Settings.Storage;
using Listen2Me.MVVM.Settings.Storage.Credentials;
using Listen2Me.WPF.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;
using Settings = Listen2Me.MVVM.Settings.Settings;

namespace Listen2Me.WPF.Modules;

public class SettingsModule : IModule
{
    public string Name { get; } = "Settings";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<ISettings, Settings>();
        
        services.AddSingleton<AppearanceSettings>();
        services.AddSingleton<StorageSettings>();
        
        services.AddSingleton<IThemeManager, ThemeManager>();
        services.AddSingleton<ICredentialSafe, CredentialSafe>();
        services.AddScoped<IConnectionStringBuilder, PostgresConnectionStringBuilder>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
    }

    public void RegisterShells(IShellRegistry registry)
    {
    }
}