using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings.Library;
using Listen2Me.MVVM.System.Browsing;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class FolderBrowserDialogViewModel : DialogViewModelBase<string>, IDropTarget
{
    [ObservableProperty] private ObservableCollection<string> _drives = new();
    [ObservableProperty] private ObservableCollection<Bookmark> _bookmarks = new();
    [ObservableProperty] private ObservableCollection<string> _folders = new();
    [ObservableProperty] private string _selectedDrive = string.Empty;
    [ObservableProperty] private Bookmark _selectedBookmark;
    [ObservableProperty] private string _selectedFolder = string.Empty;
    
    private Dictionary<string, Action> _settingsSyncMap;

    private readonly IFolderBrowser _folderBrowser;
    private readonly LibrarySettings _settings;
    
    public FolderBrowserDialogViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        IFolderBrowser folderBrowser, LibrarySettings settings)
        : base(errorHandler, logger, messenger)
    {
        _folderBrowser = folderBrowser;
        _settings = settings;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Drives = new ObservableCollection<string>(_folderBrowser.GetRoots());
        Bookmarks = new ObservableCollection<Bookmark>(_settings.Bookmarks);

        SelectedDrive = Drives[0];
        _folderBrowser.NavigateTo(SelectedDrive);
        await ActualizeSubFolders();
        
        _settingsSyncMap = new Dictionary<string, Action>()
        {
            [nameof(Bookmarks)] = () => _settings.Bookmarks = Bookmarks,
        };
        
        await base.InitializeAsync(cancellationToken);
    }

    private async Task ActualizeSubFolders()
    {
        var subFolders = await _folderBrowser.GetSubFolders();
        Folders.Clear();
        foreach (var subFolder in subFolders)
        {
            Folders.Add(subFolder);
        }
    }

    [RelayCommand]
    private async Task NavigateTo(string path)
    {
        await ExecuteSafeAsync(async _ =>
        {
            _folderBrowser.NavigateTo(path);
            await ActualizeSubFolders();
        }, "Navigate to command");
    }

    [RelayCommand]
    private async Task NavigateToChild(string path)
    {
        await ExecuteSafeAsync(async _ =>
        {
            _folderBrowser.NavigateToChild(path);
            await ActualizeSubFolders();
        }, "Navigate to child command");   
    }

    [RelayCommand]
    private void Select()
    {
        Result = string.IsNullOrEmpty(SelectedFolder) 
            ? _folderBrowser.CurrentPath 
            : Path.Combine(_folderBrowser.CurrentPath, SelectedFolder);
    }

    [RelayCommand]
    private void Cancel()
    {
        Result = null;
    }

    [RelayCommand]
    private void AddBookmark(string folder)
    {
        var path = Path.Combine(_folderBrowser.CurrentPath, folder);
        var bookmark = new Bookmark()
        {
            Id = Guid.NewGuid(),
            Path = path,
            DisplayName = folder
        };
        
        Bookmarks.Add(bookmark);
        OnPropertyChanged(nameof(Bookmarks));
    }

    [RelayCommand]
    private void RemoveBookmark(Bookmark bookmark)
    {
        Bookmarks.Remove(bookmark);
        OnPropertyChanged(nameof(Bookmarks));
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        
        if (e is not { PropertyName: { Length: > 0 } } || !IsInitialized) return;
        if (!_settingsSyncMap.TryGetValue(e.PropertyName, out var setValue)) return;
        
        setValue();
        await ExecuteSafeAsync(async ct => await _settings.SaveAsync(ct), "Save library settings");
    }

    /// <inheritdoc />
    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is not string folderName) return;

        var folder = Path.Combine(_folderBrowser.CurrentPath, folderName);
        if (!Directory.Exists(folder)) return;

        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Copy;
    }

    /// <inheritdoc />
    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is not string folderName) return;

        var folder = Path.Combine(_folderBrowser.CurrentPath, folderName);
        if (!Directory.Exists(folder)) return;

        if (Bookmarks.Any(b => b.Path == folder)) return;

        var bookmark = new Bookmark
        {
            Id = Guid.NewGuid(),
            Path = folder,
            DisplayName = folderName
        };

        Bookmarks.Add(bookmark);
        OnPropertyChanged(nameof(Bookmarks));
    }
}