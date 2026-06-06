using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Messages;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels.Components;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class MainShellViewModel : ShellViewModelBase
{
    [ObservableProperty] private NavBarViewModel _navBarViewModel;
    
    public MainShellViewModel(INavigationService navigationService, NavigationState navigationState, 
        IErrorHandler errorHandler, ILogger logger, IMessenger messenger, NavBarViewModel navBarViewModel) 
        : base(navigationService, navigationState, errorHandler, logger, messenger)
    {
        _navBarViewModel = navBarViewModel;
    }

    /// <inheritdoc />
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await NavigationService.NavigateToRouteAsync("home", cancellationToken: cancellationToken);
        
        RegisterMessage<NavBarNavigationMessage>(OnNavBarNavigationMessage);
        
        Logger.Information("MainShellViewModel initialized.");
    }

    private void OnNavBarNavigationMessage(NavBarNavigationMessage path)
    {
        NavigationService.NavigateToRouteAsync(path.Value);
        Logger.Information("Navigated to {Path}", path.Value);
    }
}