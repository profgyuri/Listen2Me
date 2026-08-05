using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Extensions;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.System.Metadata;
using Listen2Me.MVVM.ViewModels.Shells;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Layouts;

public partial class TagEditorLayoutViewModel : ViewModelBase
{
    private readonly ISharedDbContext _dbContext;
    private readonly IDialogManager _dialogManager;
    private readonly IAudioFolderScanner _folderScanner;


    [ObservableProperty] private ICollectionView _songView;
    [ObservableProperty] private ObservableCollection<Song> _songs = new();
    [ObservableProperty] private ObservableCollection<Song> _selectedSongs = new();
    [ObservableProperty] private string _filterText = string.Empty;
    
    public TagEditorLayoutViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        ISharedDbContext dbContext, IDialogManager dialogManager, IAudioFolderScanner folderScanner) 
        : base(errorHandler, logger, messenger)
    {
        _dbContext = dbContext;
        _dialogManager = dialogManager;
        _folderScanner = folderScanner;
    }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        SongView = CollectionViewSource.GetDefaultView(Songs);
        SongView.Filter = FilterSong;
        
        return base.InitializeAsync(cancellationToken);
    }

    [RelayCommand]
    private async Task AddSongsFromDb()
    {
        Logger.Information("Tag Editor: Adding songs from database");
        var songs = await _dbContext.Songs.ToListAsync();
        Songs.Clear();
        Songs.AddRange(songs);
    }

    [RelayCommand]
    private async Task AddSongsFromFolder()
    {
        Logger.Information("Tag Editor: Adding songs from folder");
        var path = await _dialogManager.ShowDialogAsync<FolderBrowserDialogViewModel, string>();
        if (string.IsNullOrEmpty(path)) return;
        
        var scanned = await _folderScanner.ScanFolderAsync(path);
        Songs.Clear();
        Songs.AddRange(scanned);
    }

    [RelayCommand]
    private void ClearGrid()
    {
        Logger.Information("Tag Editor: Clearing grid");
        Songs.Clear();
    }

    [RelayCommand]
    private async Task MoveFiles()
    {
        if (SelectedSongs.Count == 0) return;
        Logger.Information("Tag Editor: Moving files");
        var path = await _dialogManager.ShowDialogAsync<FolderBrowserDialogViewModel, string>();
        if (string.IsNullOrEmpty(path)) return;

        foreach (var song in SelectedSongs)
        {
            var fileName = Path.GetFileName(song.Path);
            var newPath = Path.Combine(path, fileName);
            File.Move(song.Path, newPath);

            song.Path = newPath;
            _dbContext.Songs.Update(song);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    partial void OnFilterTextChanged(string value)
    {
        SongView.Refresh();
    }

    private bool FilterSong(object obj)
    {
        if (obj is not Song song)
            return false;

        if (string.IsNullOrWhiteSpace(FilterText))
            return true;

        return
            song.Artist?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) == true ||
            song.Title?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) == true ||
            song.Genre?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) == true ||
            song.Path?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) == true;
    }
}