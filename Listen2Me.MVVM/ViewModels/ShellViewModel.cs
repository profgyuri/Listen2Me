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
public sealed partial class ShellViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly NavigationState _navigationState;

    [ObservableProperty] private object? _currentViewModel;

    [ObservableProperty] private string _currentRoute = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    public ShellViewModel(
        INavigationService navigationService,
        NavigationState navigationState,
        IErrorHandler errorHandler,
        ILogger logger,
        IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
        _navigationService = navigationService;
        _navigationState = navigationState;
        CurrentRoute = _navigationState.CurrentRoute;
        CurrentViewModel = _navigationState.CurrentViewModel;
        _navigationState.PropertyChanged += OnNavigationStateChanged;
    }

    [RelayCommand]
    private Task NavigateAsync(string route) =>
        ExecuteSafeAsync(
            ct => _navigationService.NavigateAsync(route, cancellationToken: ct),
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