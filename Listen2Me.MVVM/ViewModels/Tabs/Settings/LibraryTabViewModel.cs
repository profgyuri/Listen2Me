using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings.Library;
using Listen2Me.MVVM.ViewModels.Shells;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Tabs.Settings;

public partial class LibraryTabViewModel : ViewModelBase
{
    private readonly LibrarySettings _settings;
    private readonly IDialogManager _dialogManager;
    
    [ObservableProperty] private ObservableCollection<MusicFolder> _musicFolders;
    
    private Dictionary<string, Action> _settingsSyncMap;
    
    public LibraryTabViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        LibrarySettings settings, IDialogManager dialogManager) 
        : base(errorHandler, logger, messenger)
    {
        _settings = settings;
        _dialogManager = dialogManager;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _settingsSyncMap = new Dictionary<string, Action>()
        {
            [nameof(MusicFolders)] = () => _settings.MusicFolders = MusicFolders,
        };
        
        await base.InitializeAsync(cancellationToken);
    }

    [RelayCommand]
    private async Task OpenFolderBrowser()
    {
        var path = await _dialogManager.ShowDialogAsync<FolderBrowserDialogViewModel, string>();
        if (string.IsNullOrEmpty(path)) return;
        
        var folder = new MusicFolder()
        {
            Id = Guid.NewGuid(),
            Path = path,
            LastWrite = DateTime.Now
        };
        MusicFolders.Add(folder);
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e is not { PropertyName: { Length: > 0 } } || !IsInitialized) return;
        if (!_settingsSyncMap.TryGetValue(e.PropertyName, out var setValue)) return;
        
        setValue();
        await ExecuteSafeAsync(async ct => await _settings.SaveAsync(ct), "Save library settings");
    }
}