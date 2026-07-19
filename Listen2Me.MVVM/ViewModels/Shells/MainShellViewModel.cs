using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Messages;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Settings;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class MainShellViewModel : ShellViewModelBase
{
    private readonly ISettings _settings;
    
    [ObservableProperty] private WindowState _windowState;
    [ObservableProperty] private FontFamily _fontFamily;
    [ObservableProperty] private double _fontSize;
    [ObservableProperty] private bool _isBold;
    [ObservableProperty] private bool _isItalic;
    
    public MainShellViewModel(INavigationService navigationService, NavigationState navigationState, 
        IErrorHandler errorHandler, ILogger logger, IMessenger messenger, ISettings settings) 
        : base(navigationService, navigationState, errorHandler, logger, messenger)
    {
        _settings = settings;
    }

    /// <inheritdoc />
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _settings.LoadAsync(cancellationToken).ConfigureAwait(false);
        
        var accent = _settings.Appearance.Accent;
        var theme = _settings.Appearance.Theme;
        _settings.Appearance.ThemeManager.SetAccent(accent);
        _settings.Appearance.ThemeManager.SetTheme(theme);
        FontFamily = _settings.Appearance.FontFamily;
        FontSize = _settings.Appearance.FontSize;
        IsBold = _settings.Appearance.IsBold;
        IsItalic = _settings.Appearance.IsItalic;
        
        await NavigationService.NavigateToRouteAsync("home", cancellationToken: cancellationToken);
        
        RegisterMessage<NavBarNavigationMessage>(OnNavBarNavigationMessage);
        RegisterMessage<FontSettingsChangedMessage>(_ => OnFontSettingsChangedMessage());
        
        Logger.Information("MainShellViewModel initialized.");
    }

    private void OnNavBarNavigationMessage(NavBarNavigationMessage path)
    {
        NavigationService.NavigateToRouteAsync(path.Value);
        Logger.Information("Navigated to {Path}", path.Value);
    }

    private void OnFontSettingsChangedMessage()
    {
        if (!Equals(_settings.Appearance.FontFamily, FontFamily))
        {
            FontFamily = _settings.Appearance.FontFamily;
            Logger.Information("FontFamily changed to {0}", FontFamily);
        }

        if (!Equals(_settings.Appearance.FontSize, FontSize))
        {
            FontSize = _settings.Appearance.FontSize;
            Logger.Information("FontSize changed to {0}", FontSize);
        }
        
        if (!Equals(_settings.Appearance.IsBold, IsBold))
        {
            IsBold = _settings.Appearance.IsBold;
            Logger.Information("FontWeight is bold: {0}", IsBold);
        }
        
        if (!Equals(_settings.Appearance.IsItalic, IsItalic))
        {
            IsItalic = _settings.Appearance.IsItalic;
            Logger.Information("FontStyle is italic: {0}", IsItalic);
        }
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