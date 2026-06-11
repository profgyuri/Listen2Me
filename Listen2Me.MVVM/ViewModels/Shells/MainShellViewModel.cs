using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Messages;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Settings.Appearance.Themes;
using Listen2Me.MVVM.ViewModels.Components;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class MainShellViewModel : ShellViewModelBase
{
    private readonly IThemeManager _themeManager;
    
    [ObservableProperty] private NavBarViewModel _navBarViewModel;
    [ObservableProperty] private WindowState _windowState;
    
    public MainShellViewModel(INavigationService navigationService, NavigationState navigationState, 
        IErrorHandler errorHandler, ILogger logger, IMessenger messenger, NavBarViewModel navBarViewModel, 
        IThemeManager themeManager) 
        : base(navigationService, navigationState, errorHandler, logger, messenger)
    {
        _navBarViewModel = navBarViewModel;
        _themeManager = themeManager;
    }

    /// <inheritdoc />
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _themeManager.SetAccent(Accents.Blue);
        _themeManager.SetTheme(Themes.Dark);
        
        await NavigationService.NavigateToRouteAsync("home", cancellationToken: cancellationToken);
        
        RegisterMessage<NavBarNavigationMessage>(OnNavBarNavigationMessage);
        
        Logger.Information("MainShellViewModel initialized.");
    }

    private void OnNavBarNavigationMessage(NavBarNavigationMessage path)
    {
        NavigationService.NavigateToRouteAsync(path.Value);
        Logger.Information("Navigated to {Path}", path.Value);
    }

    #region Window State Buttons
    [RelayCommand]
    private void Close()
    {
        Application.Current.Shutdown();
    }
    
    [RelayCommand]
    private void Minimize()
    {
        if (WindowState is not WindowState.Minimized)
        {
            WindowState = WindowState.Minimized;
        }
    }
    
    [RelayCommand]
    private void Maximize()
    {
        if (WindowState is not WindowState.Maximized)
        {
            WindowState = WindowState.Maximized;
            return;
        }

        if (WindowState is WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
    }
    #endregion
}