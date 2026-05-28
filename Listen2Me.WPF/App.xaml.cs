using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Threading;
using Listen2Me.MVVM.ViewModels;
using Listen2Me.WPF.Threading;
using Listen2Me.WPF.Views.Shells;

namespace Listen2Me.WPF;

/// <summary>
/// Defines WPF application startup and composition root behavior.
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    /// <inheritdoc />
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            _host = CreateHostBuilder().Build();
            await _host.StartAsync().ConfigureAwait(true);

            RegisterNavigation(_host.Services);

            var window = _host.Services.GetRequiredService<MainShell>();
            window.Show();

            var navigationService = _host.Services.GetRequiredService<INavigationService>();
            var navigationOptions = _host.Services.GetRequiredService<IOptions<NavigationOptions>>().Value;
            await navigationService.NavigateAsync(navigationOptions.DefaultRoute).ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Application startup failed.");
            Shutdown(-1);
        }
    }

    /// <inheritdoc />
    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            try
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            finally
            {
                _host.Dispose();
            }
        }

        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<NavigationOptions>(context.Configuration.GetSection("Navigation"));

                var moduleCatalogOptions = context.Configuration
                    .GetSection("Modules")
                    .Get<ModuleCatalogOptions>() ?? new ModuleCatalogOptions();

                services.AddSingleton(moduleCatalogOptions);
                services.AddSingleton<INavigationRegistry, NavigationRegistry>();
                services.AddSingleton<NavigationState>();
                services.AddSingleton<IInitializationTracker, InitializationTracker>();
                services.AddScoped<INavigationService, NavigationService>();
                services.AddSingleton<IErrorHandler, LoggingErrorHandler>();
                services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
                services.AddSingleton<Serilog.ILogger>(_ => Log.Logger);
                services.AddSingleton<ShellViewModelBase>();
                services.AddSingleton<MainShell>();
                services.AddSingleton<IUiDispatcher>(_ => new WpfUiDispatcher(Application.Current.Dispatcher));

                var discoveredModules =
                    ModuleCatalog.DiscoverModules(
                        moduleCatalogOptions,
                        Log.Logger.ForContext<ModuleCatalog>());
                foreach (var module in discoveredModules)
                {
                    module.RegisterServices(services);
                }

                services.AddSingleton<IModuleCatalog>(new ModuleCatalog(discoveredModules));
            });

    private static void RegisterNavigation(IServiceProvider services)
    {
        var moduleCatalog = services.GetRequiredService<IModuleCatalog>();
        var navigationRegistry = services.GetRequiredService<INavigationRegistry>();

        foreach (var module in moduleCatalog.LoadModules())
        {
            module.RegisterNavigation(navigationRegistry);
        }
    }

    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Unhandled dispatcher exception.");
        e.Handled = true;
    }

    private static void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            Log.Fatal(exception, "Unhandled AppDomain exception.");
        }
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Unobserved task exception.");
        e.SetObserved();
    }
}