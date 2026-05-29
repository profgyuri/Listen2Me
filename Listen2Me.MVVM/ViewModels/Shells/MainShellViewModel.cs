using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Navigation;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public class MainShellViewModel : ShellViewModelBase
{
    public MainShellViewModel(INavigationService navigationService, NavigationState navigationState, 
        IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(navigationService, navigationState, errorHandler, logger, messenger)
    {
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await NavigationService.NavigateToRouteAsync("home", cancellationToken: cancellationToken);
        Logger.Information("MainShellViewModel initialized.");
    }
}