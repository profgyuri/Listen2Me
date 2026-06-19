using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Messages;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Settings;
using Listen2Me.MVVM.Settings.Appearance.Themes;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class MainShellViewModel : ShellViewModelBase
{
    private readonly ISettings _settings;
    
    [ObservableProperty] private WindowState _windowState;
    
    public MainShellViewModel(INavigationService navigationService, NavigationState navigationState, 
        IErrorHandler errorHandler, ILogger logger, IMessenger messenger, ISettings settings) 
        : base(navigationService, navigationState, errorHandler, logger, messenger)
    {
        _settings = settings;
    }

    /// <inheritdoc />
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _settings.Appearance.ThemeManager.SetAccent(Accents.Blue);
        _settings.Appearance.ThemeManager.SetTheme(Themes.Dark);
        
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