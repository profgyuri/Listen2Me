using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Messages;
using Listen2Me.MVVM.Settings;
using Listen2Me.MVVM.Settings.Appearance;
using Listen2Me.MVVM.Settings.Appearance.Themes;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Tabs.Settings;

public partial class AppearanceTabViewModel : ViewModelBase
{
    private readonly AppearanceSettings _settings;
    
    [ObservableProperty] private ObservableCollection<FontFamily> _fontFamilies;
    [ObservableProperty] private FontFamily _selectedFontFamily;
    [ObservableProperty] private ObservableCollection<Themes> _themes;
    [ObservableProperty] private Themes _selectedTheme;
    [ObservableProperty] private ObservableCollection<Accents> _accentColors;
    [ObservableProperty] private Accents _selectedAccentColor;
    [ObservableProperty] private bool _isGridEditable;
    
    public AppearanceTabViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        AppearanceSettings settings) 
        : base(errorHandler, logger, messenger)
    {
        _settings = settings;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        FontFamilies = new ObservableCollection<FontFamily>(Fonts.SystemFontFamilies);
        SelectedFontFamily = _settings.FontFamily;
        
        Themes = new ObservableCollection<Themes>(Enum.GetValues<Themes>());
        SelectedTheme = _settings.Theme;
        
        AccentColors = new ObservableCollection<Accents>(Enum.GetValues<Accents>());
        SelectedAccentColor = _settings.Accent;
        
        IsGridEditable = _settings.IsGridEditable;
        
        await base.InitializeAsync(cancellationToken);
    }

    // ReSharper disable AsyncVoidMethod
    async partial void OnSelectedFontFamilyChanged(FontFamily value)
    {
        await ExecuteSafeAsync(async (ct) =>
        {
            if (value.Equals(_settings.FontFamily)) return;
            
            _settings.FontFamily = value;
            await _settings.SaveAsync(ct);
            Messenger.Send<FontFamily>();
        }, "Change font family");
    }

    async partial void OnSelectedThemeChanged(Themes value)
    {
        await ExecuteSafeAsync(async (ct) =>
        {
            if (value.Equals(_settings.Theme)) return;
            
            _settings.Theme = value;
            await _settings.SaveAsync(ct);
        }, "Change theme");

    }

    async partial void OnSelectedAccentColorChanged(Accents value)
    {
        await ExecuteSafeAsync(async (ct) =>
        {
            if (value.Equals(_settings.Accent)) return;
            
            _settings.Accent = value;
            await _settings.SaveAsync(ct);
        }, "Change accent color");
    }

    async partial void OnIsGridEditableChanged(bool value)
    {
        await ExecuteSafeAsync(async (ct) =>
        {
            _settings.IsGridEditable = value;
            await _settings.SaveAsync(ct);
            Messenger.Send<IsGridEditableChangedMessage>();
        }, "Change grid editable");
    }
}