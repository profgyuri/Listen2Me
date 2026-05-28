# MVVM Scaffolding Guide

This document explains how to use the MVVM scaffolding built with `CommunityToolkit.MVVM`, Generic Host, and Serilog.

Primary contracts:
- `Listen2Me.MVVM/ViewModels/ViewModelBase.cs`
- `Listen2Me.MVVM/Initialization/IInitializeAsync.cs`
- `Listen2Me.MVVM/Navigation/INavigationService.cs`
- `Listen2Me.MVVM/Modules/IModule.cs`

Host integration:
- `Listen2Me.WPF/App.xaml.cs`

## Purpose

The scaffold gives you a consistent enterprise baseline for WPF MVVM apps:
- VM-first navigation by route
- one-time async ViewModel initialization
- module-based composition (static + optional assembly scan)
- centralized error handling
- Serilog-based logging everywhere in the MVVM layer
- CommunityToolkit source generators for properties/commands

## Key Concepts

### ViewModel base

All app ViewModels should inherit:
- `ViewModelBase : ObservableObject, IInitializeAsync`

`ViewModelBase` provides:
- `IsInitialized`
- `InitializeAsync(...)` (virtual)
- `EnsureInitializedAsync(...)` (runs once per VM instance)
- `ExecuteSafeAsync(...)` (centralized exception + Serilog + `IErrorHandler`)
- `RegisterMessage<TMessage>(...)` and automatic cleanup on `Dispose()`

### Initialization lifecycle

Initialization runs **once per ViewModel instance**.

Navigation uses:
- `INavigationService.NavigateAsync(route, parameter, ct)`

Before it sets the active VM in `NavigationState`, it:
1. resolves the VM from DI
2. checks `IInitializeAsync`
3. calls one-time initialization through `IInitializationTracker`

### Navigation model

Routes are registered in modules:
- `INavigationRegistry.Register<TViewModel>("route")`

Shell state is held in:
- `NavigationState.CurrentRoute`
- `NavigationState.CurrentViewModel`

The shell view binds to `CurrentViewModel` and uses DataTemplates to map VM types to views.

### Modules

Modules implement:
- `IModule.RegisterServices(IServiceCollection)`
- `IModule.RegisterNavigation(INavigationRegistry)`

Discovery is hybrid:
- automatic assembly scanning via `ModuleCatalogOptions`

## When To Use This Scaffold

Use it when you need:
- a maintainable WPF architecture with DI and navigation
- async startup logic in VMs (loading state, warm-up, cache hydration)
- modular feature composition
- consistent logging and error flow

Avoid it when:
- your app is very small (single window, no navigation/modules)
- you do not need DI or startup orchestration

## Lifecycle Pattern (Recommended)

1. Define a ViewModel inheriting `ViewModelBase`.
2. Override `InitializeAsync` only for real async startup work.
3. Create a module implementing `IModule`.
4. Register VM/service types in `RegisterServices`.
5. Register routes in `RegisterNavigation`.
6. Ensure module assembly name matches configured scan prefixes.
7. Add a DataTemplate mapping VM -> View.
8. Navigate by route from shell/commands.

## Example: Feature Module With Async Initialization

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels;

public interface IReportService
{
    Task<int> CountAsync(CancellationToken ct);
}

public sealed class ReportModule : IModule
{
    public string Name => "Reports";

    public void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<ReportsViewModel>();
        services.AddSingleton<IReportService, ReportService>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
        registry.Register<ReportsViewModel>("reports");
    }
}

public sealed partial class ReportsViewModel : ViewModelBase
{
    private readonly IReportService _reportService;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private int _reportCount;

    public ReportsViewModel(
        IReportService reportService,
        IErrorHandler errorHandler,
        ILogger logger,
        IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
        _reportService = reportService;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // Runs once per VM instance when first navigated to.
        ReportCount = await _reportService.CountAsync(cancellationToken);
    }

    [RelayCommand]
    private Task RefreshAsync() =>
        ExecuteSafeAsync(async ct =>
        {
            IsBusy = true;
            ReportCount = await _reportService.CountAsync(ct);
            IsBusy = false;
        }, "Reports.Refresh");
}
```

## Complete `INavigationService` Example (All Required Pieces)

This is the minimum complete setup to navigate to `"reports"` from another ViewModel and have it render in the shell.

### 1. Target ViewModel + route registration

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels;

public sealed class ReportsModule : IModule
{
    public string Name => "Reports";

    public void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<ReportsViewModel>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
        registry.Register<ReportsViewModel>("reports");
    }
}

public sealed partial class ReportsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Reports";

    public ReportsViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
    }
}
```

### 2. Ensure module discovery can find your module

```csharp
// appsettings.json
{
  "Modules": {
    "EnableAssemblyScan": true,
    "ScanAssemblyPrefixes": [ "Listen2Me." ]
  }
}
```

```csharp
// Module must be public and have a parameterless constructor.
public sealed class ReportsModule : IModule
{
    public string Name => "Reports";
    // ...
}
```

### 3. Navigate from another ViewModel using `INavigationService`

```csharp
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels;

public sealed partial class MenuViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    public MenuViewModel(
        INavigationService navigation,
        IErrorHandler errorHandler,
        ILogger logger,
        IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
        _navigation = navigation;
    }

    [RelayCommand]
    private Task OpenReportsAsync() =>
        ExecuteSafeAsync(
            ct => _navigation.NavigateAsync("reports", cancellationToken: ct),
            "Menu.OpenReports");
}
```

### 4. Map ViewModel to View so WPF can render it

```xml
<Application.Resources>
  <DataTemplate DataType="{x:Type viewModels:ReportsViewModel}">
    <views:ReportsView />
  </DataTemplate>
</Application.Resources>
```

### 5. Ensure shell content host is bound to `CurrentViewModel`

```xml
<ContentControl Content="{Binding CurrentViewModel}" />
```

Once these pieces are in place, calling `NavigateAsync("reports")`:
1. resolves `ReportsViewModel` from DI,
2. runs one-time `InitializeAsync` if overridden,
3. updates `NavigationState.CurrentViewModel`,
4. and WPF swaps the rendered view via the DataTemplate.

## Brief Messaging Example (`WeakReferenceMessenger`)

Use messaging when two ViewModels should communicate without direct references.

```csharp
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Serilog;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.ViewModels;

// Shared message type
public sealed class StatusMessage : ValueChangedMessage<string>
{
    public StatusMessage(string value) : base(value) { }
}

// Sender VM
public sealed class ProducerViewModel : ViewModelBase
{
    public ProducerViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger)
        : base(errorHandler, logger, messenger) { }

    public void Publish() => Messenger.Send(new StatusMessage("Background sync finished."));
}

// Receiver VM
public sealed partial class ConsumerViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _statusText = "Waiting...";

    public ConsumerViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
        RegisterMessage<StatusMessage>(m => StatusText = m.Value);
    }
}
```

Tip: subscriptions registered via `RegisterMessage<TMessage>` are automatically removed when the ViewModel is disposed.

## Serilog Integration Notes

- MVVM components use `Serilog.ILogger` directly.
- The WPF host registers `ILogger` in DI (`Log.Logger`).
- `UseSerilog(...)` in host startup controls sinks/enrichment/levels.
- `IErrorHandler` receives exceptions from navigation and ViewModel safe execution paths.

## Best Practices

- Keep route keys stable and unique.
- Keep `InitializeAsync` idempotent and cancellation-aware.
- Use `ExecuteSafeAsync` for command bodies that can fail.
- Prefer module boundaries for feature areas, not per-screen micro-modules.
- Fail fast on duplicate module names/routes.

## Common Pitfalls

- Doing heavy work in constructors instead of `InitializeAsync`.
- Forgetting to register the route for a ViewModel.
- Assuming `InitializeAsync` runs on every navigation (it does not; once per instance).
- Bypassing DI and manually creating VMs (breaks navigation/init conventions).

## Included Example Module

The WPF app now includes a concrete example module that is discovered automatically by scanning:
- Module: `Listen2Me.WPF/Modules/Example/ExampleModule.cs`
- ViewModel: `Listen2Me.WPF/Modules/Example/ExampleModuleViewModel.cs`
- View: `Listen2Me.WPF/Views/ExampleModuleView.xaml`
- Route: `"example"`

What it demonstrates:
- module service registration (`IExampleStatusService`)
- route registration (`registry.Register<ExampleModuleViewModel>("example")`)
- one-time async initialization (`InitializeAsync`)
- command-driven refresh (`RefreshStatusCommand`)
- simple messenger publish/receive using `RegisterMessage<TMessage>`

## Full Walkthrough: Add a Nested UserControl With Its Own ViewModel

Yes, you can nest a UserControl and give it its own ViewModel in this architecture.  
The clean pattern is: **parent VM owns child VM**, and parent view renders child VM using `ContentControl` + `DataTemplate`.

### 1) Create the child ViewModel

Create `ChildWidgetViewModel` inheriting `ViewModelBase`.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.ViewModels;

public sealed partial class ChildWidgetViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _header = "Child Widget";

    [ObservableProperty]
    private string _details = "Ready";

    public ChildWidgetViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
    }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Details = "Initialized from child VM";
        return Task.CompletedTask;
    }
}
```

### 2) Register child ViewModel in module DI

In your module `RegisterServices` (for example in `ExampleModule.cs`):

```csharp
public void RegisterServices(IServiceCollection services)
{
    services.AddSingleton<IExampleStatusService, ExampleStatusService>();
    services.AddTransient<ExampleModuleViewModel>();
    services.AddTransient<ChildWidgetViewModel>(); // add this
}
```

### 3) Inject child ViewModel into parent ViewModel

In parent VM (`ExampleModuleViewModel`), expose child VM as a property.

```csharp
public sealed partial class ExampleModuleViewModel : ViewModelBase
{
    public ChildWidgetViewModel ChildWidget { get; }

    public ExampleModuleViewModel(
        ChildWidgetViewModel childWidget,
        IExampleStatusService statusService,
        IErrorHandler errorHandler,
        ILogger logger,
        IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
        ChildWidget = childWidget;
        // existing setup...
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // Parent gets initialized by navigation service.
        // Child is not navigated directly, so initialize it explicitly.
        await ChildWidget.EnsureInitializedAsync(cancellationToken);
        // existing parent init...
    }
}
```

### 4) Create child UserControl view

Create `ChildWidgetView.xaml` as a normal `UserControl` and bind to child VM properties.

```xml
<UserControl x:Class="Listen2Me.WPF.Views.ChildWidgetView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Border Padding="10" BorderBrush="#22000000" BorderThickness="1">
    <StackPanel>
      <TextBlock FontWeight="Bold" Text="{Binding Header}" />
      <TextBlock Margin="0,4,0,0" Text="{Binding Details}" />
    </StackPanel>
  </Border>
</UserControl>
```

### 5) Map child ViewModel to child view via DataTemplate

In `App.xaml`, add a DataTemplate:

```xml
<DataTemplate DataType="{x:Type exampleViewModels:ChildWidgetViewModel}">
  <views:ChildWidgetView />
</DataTemplate>
```

Make sure the `xmlns` for `exampleViewModels` points to the namespace containing `ChildWidgetViewModel`.

### 6) Render child ViewModel inside parent view

In `ExampleModuleView.xaml`, add:

```xml
<ContentControl Content="{Binding ChildWidget}" />
```

That is all WPF needs: it sees the child VM instance, matches the DataTemplate, and renders `ChildWidgetView`.

### 7) Runtime flow summary

1. `INavigationService` navigates to parent route and initializes parent VM once.  
2. Parent VM calls `ChildWidget.EnsureInitializedAsync(...)`.  
3. Parent view binds `ContentControl` to `ChildWidget`.  
4. WPF applies child DataTemplate and displays child UserControl.

This keeps everything testable, DI-driven, and aligned with your existing MVVM conventions.

## Deep-Link Behavior for Toggleable Widgets

If you want URLs/routes like:
- `example/summary`
- `example/details`

then the widget state should be route-driven (not only button-driven).

### Approach

Register **two routes** that resolve to **two parent ViewModels**:
- both use the same parent view (`ExampleModuleView`)
- each sets a different initial widget mode during initialization

This gives you deterministic deep links while still allowing in-view toggles later.

### 1) Register two route ViewModels

```csharp
public sealed class ExampleModule : IModule
{
    public string Name => "Example";

    public void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<ExampleSummaryPageViewModel>();
        services.AddTransient<ExampleDetailsPageViewModel>();
        services.AddTransient<SummaryWidgetViewModel>();
        services.AddTransient<DetailsWidgetViewModel>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
        registry.Register<ExampleSummaryPageViewModel>("example/summary");
        registry.Register<ExampleDetailsPageViewModel>("example/details");
    }
}
```

### 2) Share one parent view model base

```csharp
public abstract partial class ExamplePageViewModelBase : ViewModelBase
{
    public SummaryWidgetViewModel Summary { get; }
    public DetailsWidgetViewModel Details { get; }

    [ObservableProperty]
    private object? _currentWidget;

    protected ExamplePageViewModelBase(
        SummaryWidgetViewModel summary,
        DetailsWidgetViewModel details,
        IErrorHandler errorHandler,
        ILogger logger,
        IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
        Summary = summary;
        Details = details;
    }

    protected async Task InitializeWidgetsAsync(CancellationToken ct)
    {
        await Summary.EnsureInitializedAsync(ct);
        await Details.EnsureInitializedAsync(ct);
    }
}

public sealed class ExampleSummaryPageViewModel : ExamplePageViewModelBase
{
    public ExampleSummaryPageViewModel(
        SummaryWidgetViewModel summary,
        DetailsWidgetViewModel details,
        IErrorHandler errorHandler,
        ILogger logger,
        IMessenger messenger)
        : base(summary, details, errorHandler, logger, messenger) { }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await InitializeWidgetsAsync(cancellationToken);
        CurrentWidget = Summary; // deep-link target
    }
}

public sealed class ExampleDetailsPageViewModel : ExamplePageViewModelBase
{
    public ExampleDetailsPageViewModel(
        SummaryWidgetViewModel summary,
        DetailsWidgetViewModel details,
        IErrorHandler errorHandler,
        ILogger logger,
        IMessenger messenger)
        : base(summary, details, errorHandler, logger, messenger) { }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await InitializeWidgetsAsync(cancellationToken);
        CurrentWidget = Details; // deep-link target
    }
}
```

### 3) Map both route VMs to the same parent view

```xml
<DataTemplate DataType="{x:Type exampleViewModels:ExampleSummaryPageViewModel}">
  <views:ExampleModuleView />
</DataTemplate>
<DataTemplate DataType="{x:Type exampleViewModels:ExampleDetailsPageViewModel}">
  <views:ExampleModuleView />
</DataTemplate>
```

The view binds to:

```xml
<ContentControl Content="{Binding CurrentWidget}" />
```

### 4) Navigate via route for deep links

```csharp
await _navigationService.NavigateAsync("example/summary");
await _navigationService.NavigateAsync("example/details");
```

Result:
- opening `"example/summary"` lands directly on Summary widget
- opening `"example/details"` lands directly on Details widget
- both still support in-page toggling if you add toggle commands/buttons

