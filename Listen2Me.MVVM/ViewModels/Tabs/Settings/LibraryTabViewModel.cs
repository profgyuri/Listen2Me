using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings.Library;
using Listen2Me.MVVM.System.Metadata;
using Listen2Me.MVVM.ViewModels.Shells;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Tabs.Settings;

public partial class LibraryTabViewModel : ViewModelBase
{
    private readonly LibrarySettings _settings;
    private readonly IDialogManager _dialogManager;
    private readonly IServiceScopeFactory _scopeFactory;
    
    [ObservableProperty] private ObservableCollection<MusicFolder> _musicFolders = new();
    [ObservableProperty] private ObservableCollection<MusicFolder> _selectedMusicFolders = new();
    [ObservableProperty] private bool _scanAutomatically;
    [ObservableProperty] private bool _isScanning;
    [ObservableProperty] private int _scanProgressPercentage;
    [ObservableProperty] private string _scanProgress = "Scanning has not started yet";
    
    private Dictionary<string, Action> _settingsSyncMap;
    private CancellationTokenSource? _scanCts;
    
    public LibraryTabViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        LibrarySettings settings, IDialogManager dialogManager, IServiceScopeFactory scopeFactory) 
        : base(errorHandler, logger, messenger)
    {
        _settings = settings;
        _dialogManager = dialogManager;
        _scopeFactory = scopeFactory;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _settingsSyncMap = new Dictionary<string, Action>()
        {
            [nameof(MusicFolders)] = () => _settings.MusicFolders = MusicFolders,
            [nameof(ScanAutomatically)] = () => _settings.ScanAutomatically = ScanAutomatically,
        };
        
        MusicFolders = new ObservableCollection<MusicFolder>(_settings.MusicFolders);
        ScanAutomatically = _settings.ScanAutomatically;
        
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
        OnPropertyChanged(nameof(MusicFolders));
    }

    [RelayCommand]
    private void RemoveSelected()
    {
        foreach (var selectedMusicFolder in SelectedMusicFolders)
        {
            MusicFolders.Remove(selectedMusicFolder);
        }
        OnPropertyChanged(nameof(MusicFolders));
    }

    [RelayCommand]
    private async Task Scan()
    {
        Logger.Information("Scan initiated");
        if (_scanCts is not null) return;
        IsScanning = true;
        _scanCts = new CancellationTokenSource();
        var progressReporter = new Progress<int>(value =>
        {
            ScanProgressPercentage = value;
            ScanProgress = $"{value}%";
        });
        
        ScanProgress = "0%";
        ScanProgressPercentage = 0;
        
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var audioFolderScanner = scope.ServiceProvider.GetRequiredService<IAudioFolderScanner>();
            await audioFolderScanner.ScanFoldersAsync(progressReporter, _scanCts.Token);
            ScanProgress = "Scan completed";
        }
        catch (OperationCanceledException e)
        {
            Logger.Information("Scan cancelled by user");
            ScanProgress = "Scan cancelled";
        }
        
        _scanCts = null;
        IsScanning = false;
    }

    [RelayCommand]
    private async Task CancelScan()
    {
        if (_scanCts is null || _scanCts.IsCancellationRequested) return;
        await _scanCts.CancelAsync();
        _scanCts = null;
        
        IsScanning = false;
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