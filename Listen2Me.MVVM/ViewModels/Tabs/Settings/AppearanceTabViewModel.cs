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

public partial class AppearanceTabViewModel : SyncableSettingsViewModel<AppearanceSettings>
{
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
    
    public AppearanceTabViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        AppearanceSettings settings) 
        : base(errorHandler, logger, messenger, settings)
    { }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        SyncProperty(nameof(IsBold), () => Settings.IsBold = IsBold);
        SyncProperty(nameof(IsItalic), () => Settings.IsItalic = IsItalic);
        SyncProperty(nameof(SelectedFontSize), () => Settings.FontSize = SelectedFontSize);
        SyncProperty(nameof(SelectedAccentColor), () => Settings.Accent = SelectedAccentColor);
        SyncProperty(nameof(SelectedTheme), () => Settings.Theme = SelectedTheme);
        SyncProperty(nameof(SelectedFontFamily), () => Settings.FontFamily = SelectedFontFamily);
        
        SelectedFontFamily = Settings.FontFamily;
        FontSizes = [10, 11, 12, 13, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40];
        SelectedFontSize = Settings.FontSize;
        IsBold = Settings.IsBold;
        IsItalic = Settings.IsItalic;
        
        Themes = new ObservableCollection<Themes>(Enum.GetValues<Themes>());
        SelectedTheme = Settings.Theme;
        
        AccentColors = new ObservableCollection<Accents>(Enum.GetValues<Accents>());
        SelectedAccentColor = Settings.Accent;
        
        IsGridEditable = Settings.IsGridEditable;
        
        await base.InitializeAsync(cancellationToken);
    }

    async partial void OnIsGridEditableChanged(bool value)
    {
        await ExecuteSafeAsync(async ct =>
        {
            Settings.IsGridEditable = value;
            await Settings.SaveAsync(ct);
            Messenger.Send<IsGridEditableChangedMessage>();
        }, "Change grid editable");
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        Messenger.Send<FontSettingsChangedMessage>();
    }
}