using Microsoft.Extensions.DependencyInjection;
using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;

namespace Listen2Me.WPF.Modules.Example;

/// <summary>
/// Registers services and navigation for the example module.
/// </summary>
public sealed class ExampleModule : IModule
{
    /// <inheritdoc />
    public string Name => "Example";

    /// <inheritdoc />
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IExampleStatusService, ExampleStatusService>();
        services.AddTransient<ExampleModuleViewModel>();
    }

    /// <inheritdoc />
    public void RegisterNavigation(INavigationRegistry registry)
    {
        registry.Register<ExampleModuleViewModel>("example");
    }
}