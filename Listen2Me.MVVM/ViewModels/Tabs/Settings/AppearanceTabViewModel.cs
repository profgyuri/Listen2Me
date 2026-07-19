using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Messages;
using Listen2Me.MVVM.Settings.Appearance;
using Listen2Me.MVVM.Settings.Appearance.Themes;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Tabs.Settings;

public partial class AppearanceTabViewModel : ViewModelBase
{
    private readonly AppearanceSettings _settings;
    
    [ObservableProperty] private FontFamily _selectedFontFamily;
    [ObservableProperty] private double _selectedFontSize;
    [ObservableProperty] private IEnumerable<double> _fontSizes;
    [ObservableProperty] private ObservableCollection<Themes> _themes;
    [ObservableProperty] private bool _isBold;
    [ObservableProperty] private bool _isItalic;
    [ObservableProperty] private Themes _selectedTheme;
    [ObservableProperty] private ObservableCollection<Accents> _accentColors;
    [ObservableProperty] private Accents _selectedAccentColor;
    [ObservableProperty] private bool _isGridEditable;
    
    private Dictionary<string, Action> _settingsSyncMap;
    
    public AppearanceTabViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        AppearanceSettings settings) 
        : base(errorHandler, logger, messenger)
    {
        _settings = settings;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _settingsSyncMap = new Dictionary<string, Action>
        {
            [nameof(IsBold)] = () => _settings.IsBold = IsBold,
            [nameof(IsItalic)] = () => _settings.IsItalic = IsItalic,
            [nameof(SelectedFontSize)] = () => _settings.FontSize = SelectedFontSize,
            [nameof(SelectedAccentColor)] = () => _settings.Accent = SelectedAccentColor,
            [nameof(SelectedTheme)] = () => _settings.Theme = SelectedTheme,
            [nameof(SelectedFontFamily)] = () => _settings.FontFamily = SelectedFontFamily,
        };
        
        SelectedFontFamily = _settings.FontFamily;
        FontSizes = [10, 11, 12, 13, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40];
        SelectedFontSize = _settings.FontSize;
        IsBold = _settings.IsBold;
        IsItalic = _settings.IsItalic;
        
        Themes = new ObservableCollection<Themes>(Enum.GetValues<Themes>());
        SelectedTheme = _settings.Theme;
        
        AccentColors = new ObservableCollection<Accents>(Enum.GetValues<Accents>());
        SelectedAccentColor = _settings.Accent;
        
        IsGridEditable = _settings.IsGridEditable;
        
        await base.InitializeAsync(cancellationToken);
    }

    async partial void OnIsGridEditableChanged(bool value)
    {
        await ExecuteSafeAsync(async ct =>
        {
            _settings.IsGridEditable = value;
            await _settings.SaveAsync(ct);
            Messenger.Send<IsGridEditableChangedMessage>();
        }, "Change grid editable");
    }

    // We use this to save the settings when no custom interaction is needed
    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e is not { PropertyName: { Length: > 0 } } || !IsInitialized) return;
        if (!_settingsSyncMap.TryGetValue(e.PropertyName, out var setValue)) return;
        
        setValue();
        await ExecuteSafeAsync(async ct => await _settings.SaveAsync(ct), "Save appearance settings");
        Messenger.Send<FontSettingsChangedMessage>();
    }
}