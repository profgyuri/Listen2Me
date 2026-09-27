using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.Controllers;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Extensions;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Services;
using Listen2Me.MVVM.System;
using Listen2Me.MVVM.System.Metadata;
using Listen2Me.MVVM.ViewModels.Shells;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Layouts;

public partial class TagEditorLayoutViewModel : ViewModelBase
{
    private readonly HashSet<Song> _subscribedSongs = new();
    
    private readonly ISharedDbContext _dbContext;
    private readonly IDialogManager _dialogManager;
    private readonly IAudioFolderScanner _folderScanner;
    private readonly IMetadataWriter _metadataWriter;
    private readonly IFileRenamer _fileRenamer;
    private readonly IDragNDropController _dragNDropController;
    private readonly IFileMoverService _fileMoverService;
    private readonly IAutoTagFetcherService _tagFetcherService;

    [ObservableProperty] private ICollectionView _songView;
    [ObservableProperty] private ObservableCollection<Song> _songs = new();
    [ObservableProperty] private ObservableCollection<Song> _selectedSongs = new();
    [ObservableProperty] private string _filterText = string.Empty;
    
    public TagEditorLayoutViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        ISharedDbContext dbContext, IDialogManager dialogManager, IAudioFolderScanner folderScanner, 
        IMetadataWriter metadataWriter, IFileRenamer fileRenamer, IDragNDropController dragNDropController, 
        IFileMoverService fileMoverService, IAutoTagFetcherService tagFetcherService) 
        : base(errorHandler, logger, messenger)
    {
        _dbContext = dbContext;
        _dialogManager = dialogManager;
        _folderScanner = folderScanner;
        _metadataWriter = metadataWriter;
        _fileRenamer = fileRenamer;
        _dragNDropController = dragNDropController;
        _fileMoverService = fileMoverService;
        _tagFetcherService = tagFetcherService;
    }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        SongView = CollectionViewSource.GetDefaultView(Songs);
        SongView.Filter = FilterSong;
        
        Songs.CollectionChanged += Songs_CollectionChanged;
        foreach (var s in Songs)
        {
            s.PropertyChanged += Song_PropertyChanged;
            s.PropertyChanging += Song_PropertyChanging;
        }

        
        return base.InitializeAsync(cancellationToken);
    }

    /// <summary>
    /// Handles dropped paths, files and folders alike.
    /// </summary>
    /// <param name="paths"></param>
    public async Task HandleDroppedPaths(string[] paths)
    {
        await _dragNDropController.AddDroppedPathsToCollection(Songs, paths);
    }

    #region Commands
    
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
        await _fileMoverService.MoveFilesAsync(SelectedSongs);
    }

    [RelayCommand]
    private async Task FilenameToTags()
    {
        if (SelectedSongs.Count == 0) return;

        await _tagFetcherService.FetchTagsFromFilenameAsync(SelectedSongs);
        
        Logger.Information("Tag Editor: Filename to tags completed for {0} songs", SelectedSongs.Count);
    }
    
    [RelayCommand]
    private async Task TagsToFilename()
    {
        if (SelectedSongs.Count == 0) return;

        await _tagFetcherService.FetchFilenameFromTagsAsync(SelectedSongs);
    }

    #endregion
    
    partial void OnFilterTextChanged(string value)
    {
        SongView.Refresh();
    }

    #region Tag Data change detection

    private readonly SemaphoreSlim _propertyChangedLock = new(1, 1);
    private readonly Dictionary<Guid, string> _pathChanges = new();
    
    private void Song_PropertyChanging(object? sender, PropertyChangingEventArgs e)
    {
        var song = (Song)sender!;
        if (e.PropertyName?.Equals(nameof(Song.Path)) == true)
            _pathChanges[song.Id] = song.Path;
    }
    
    private async void Song_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var song = (Song)sender!;
        var locked = false;
        try
        {
            locked = await _propertyChangedLock.WaitAsync(TimeSpan.FromSeconds(1));
            if (!locked)
            {
                Logger.Warning("Tag Editor: Timed out waiting for lock on {0}", song.Path);
                return;
            }

            Logger.Information("Tag Editor: Song {0} changed", song.Path);
            
            var returnEarly = false;

            if (e.PropertyName?.Equals(nameof(Song.Path)) == true)
            {
                try
                {
                    _fileRenamer.Rename(song, _pathChanges[song.Id]);
                }
                catch (Exception exception)
                {
                    Logger.Warning(exception, "Tag Editor: Failed to rename file {0} to {1}", 
                        song.Path, _pathChanges[song.Id]);
                    returnEarly = true;
                }
                finally
                {
                    _pathChanges.Remove(song.Id);   
                }
            }
            
            if (returnEarly) return;

            await _metadataWriter.UpdateTags(song);
            
            var isTracked = _dbContext.Songs.Entry(song).State != EntityState.Detached;
            if (isTracked)
            {
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Tag Editor: Failed to update song tags");
        }
        finally
        {
            if (locked) _propertyChangedLock.Release();
        }
    }

    private void Songs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // This is the branched out case for the Reset event after the collection is cleared.
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var song in _subscribedSongs)
            {
                song.PropertyChanged -= Song_PropertyChanged;
                song.PropertyChanging -= Song_PropertyChanging;
            }
            _subscribedSongs.Clear();

            return;
        }
        
        if (e.OldItems is not null)
        {
            foreach (Song song in e.OldItems)
            {
                song.PropertyChanged -= Song_PropertyChanged;
                song.PropertyChanging -= Song_PropertyChanging;
                _subscribedSongs.Remove(song);
            }
        }

        if (e.NewItems is null) return;
        
        foreach (Song song in e.NewItems)
        {
            if (_subscribedSongs.Contains(song)) continue;
                
            song.PropertyChanged += Song_PropertyChanged;
            song.PropertyChanging += Song_PropertyChanging;
            _subscribedSongs.Add(song);
        }
    }

    #endregion

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