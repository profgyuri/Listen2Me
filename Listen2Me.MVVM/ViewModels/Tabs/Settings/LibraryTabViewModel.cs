using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings.Library;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Tabs.Settings;

public partial class LibraryTabViewModel : ViewModelBase
{
    private readonly LibrarySettings _settings;
    
    [ObservableProperty] private ObservableCollection<MusicFolder> _musicFolders;
    
    private Dictionary<string, Action> _settingsSyncMap;
    
    public LibraryTabViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        LibrarySettings settings) 
        : base(errorHandler, logger, messenger)
    {
        _settings = settings;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _settingsSyncMap = new Dictionary<string, Action>()
        {
            [nameof(MusicFolders)] = () => _settings.MusicFolders = MusicFolders,
        };
        
        await base.InitializeAsync(cancellationToken);
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