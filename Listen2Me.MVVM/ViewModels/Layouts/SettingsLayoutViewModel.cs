using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.ViewModels.Tabs.Settings;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Layouts;

public partial class SettingsLayoutViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentTab;
    public GeneralTabViewModel GeneralTab { get; init; }
    public AppearanceTabViewModel AppearanceTab { get; init; }
    public LibraryTabViewModel LibraryTab { get; init; }
    public PlaybackTabViewModel PlaybackTab { get; init; }
    
    public SettingsLayoutViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        GeneralTabViewModel generalTab, AppearanceTabViewModel appearanceTab, 
        LibraryTabViewModel libraryTab, PlaybackTabViewModel playbackTab) 
        : base(errorHandler, logger, messenger)
    {
        GeneralTab = generalTab;
        AppearanceTab = appearanceTab;
        LibraryTab = libraryTab;
        PlaybackTab = playbackTab;
        
        SwitchTo(GeneralTab).ConfigureAwait(false);
    }
    
    [RelayCommand]
    private async Task SwitchTo(ViewModelBase vm)
    {
        if (CurrentTab == vm) return;
        
        CurrentTab?.IsActive = false;
        CurrentTab = vm;
        CurrentTab.IsActive = true;
        await CurrentTab.EnsureInitializedAsync();
    }
}