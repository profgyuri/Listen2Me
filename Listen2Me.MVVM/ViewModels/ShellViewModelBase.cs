using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Navigation;

namespace Listen2Me.MVVM.ViewModels;

/// <summary>
/// Represents the shell view model that hosts the current navigation target.
/// </summary>
public partial class ShellViewModelBase : ViewModelBase
{
    protected readonly INavigationService NavigationService;
    private readonly NavigationState _navigationState;

    [ObservableProperty] private object? _currentViewModel;

    [ObservableProperty] private string _currentRoute = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModelBase"/> class.
    /// </summary>
    public ShellViewModelBase(
        INavigationService navigationService,
        NavigationState navigationState,
        IErrorHandler errorHandler,
        ILogger logger,
        IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
        NavigationService = navigationService;
        _navigationState = navigationState;
        CurrentRoute = _navigationState.CurrentRoute;
        CurrentViewModel = _navigationState.CurrentViewModel;
        _navigationState.PropertyChanged += OnNavigationStateChanged;
    }

    [RelayCommand]
    private Task NavigateAsync(string route) =>
        ExecuteSafeAsync(
            ct => NavigationService.NavigateToRouteAsync(route, cancellationToken: ct),
            $"Navigate({route})");

    private void OnNavigationStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NavigationState.CurrentRoute))
        {
            CurrentRoute = _navigationState.CurrentRoute;
        }

        if (e.PropertyName == nameof(NavigationState.CurrentViewModel))
        {
            CurrentViewModel = _navigationState.CurrentViewModel;
        }
    }
}