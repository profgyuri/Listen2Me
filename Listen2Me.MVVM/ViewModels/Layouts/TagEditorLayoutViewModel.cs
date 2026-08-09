using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using Listen2Me.MVVM.Settings;
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
    private readonly IMetadataReader _metadataReader;
    private readonly ISettings _settings;
    private readonly IFileRenamer _fileRenamer;

    [ObservableProperty] private ICollectionView _songView;
    [ObservableProperty] private ObservableCollection<Song> _songs = new();
    [ObservableProperty] private ObservableCollection<Song> _selectedSongs = new();
    [ObservableProperty] private string _filterText = string.Empty;
    
    public TagEditorLayoutViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        ISharedDbContext dbContext, IDialogManager dialogManager, IAudioFolderScanner folderScanner, 
        IMetadataWriter metadataWriter, IMetadataReader metadataReader, ISettings settings, IFileRenamer fileRenamer) 
        : base(errorHandler, logger, messenger)
    {
        _dbContext = dbContext;
        _dialogManager = dialogManager;
        _folderScanner = folderScanner;
        _metadataWriter = metadataWriter;
        _metadataReader = metadataReader;
        _settings = settings;
        _fileRenamer = fileRenamer;
    }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        SongView = CollectionViewSource.GetDefaultView(Songs);
        SongView.Filter = FilterSong;
        
        Songs.CollectionChanged += Songs_CollectionChanged;
        foreach (var s in Songs)
            s.PropertyChanged += Song_PropertyChanged;

        
        return base.InitializeAsync(cancellationToken);
    }

    /// <summary>
    /// Handles dropped paths, files and folders alike.
    /// </summary>
    /// <param name="paths"></param>
    public async Task HandleDroppedPaths(string[] paths)
    {
        foreach (var path in paths)
        {
            try
            {
                if (File.Exists(path) && 
                    Constants.SupportedAudioExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    var toAdd = await _dbContext.Songs.FirstOrDefaultAsync(s => s.Path == path);
                    if (toAdd is null)
                    {
                        toAdd = _metadataReader.Read(path);
                    }
                    
                    if (!Songs.Contains(toAdd))
                        Songs.Add(toAdd);
                }
                else if (Directory.Exists(path))
                {
                    List<Song> toAdd;
                    if (_settings.Library.MusicFolders.Any(x => x.Path.Equals(path, StringComparison.OrdinalIgnoreCase)))
                    {
                        toAdd = await _dbContext.Songs.Where(s => 
                                EF.Functions.Like(s.Path, path + "%")).ToListAsync();
                    }
                    else
                    {
                        toAdd = await _folderScanner.ScanFolderAsync(path);
                    }

                    toAdd = toAdd.Except(Songs).ToList();
                    Songs.AddRange(toAdd);
                }
                else
                {
                    Logger.Warning("Dropped path {Path} is not supported", path);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to handle dropped path {Path}", path);
            }
        }
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

    #endregion
    
    partial void OnFilterTextChanged(string value)
    {
        SongView.Refresh();
    }

    #region Tag Data change detection

    partial void OnSongsChanging(ObservableCollection<Song>? oldValue, ObservableCollection<Song> newValue)
    {
        Logger.Information("Tag Editor: Songs changing");
        if (oldValue is null) return;
        
        oldValue.CollectionChanged -= Songs_CollectionChanged;
        foreach (var s in oldValue)
            s.PropertyChanged -= Song_PropertyChanged;
    }
    
    partial void OnSongsChanged(ObservableCollection<Song> value)
    {
        Logger.Information("Tag Editor: Songs changed");
        value.CollectionChanged += Songs_CollectionChanged;
        foreach (var s in value)
            s.PropertyChanged += Song_PropertyChanged;

        Logger.Debug("Tag Editor: Songs count: {0}", value.Count);
        SongView = CollectionViewSource.GetDefaultView(value);
    }

    private readonly SemaphoreSlim _propertyChangedLock = new(1, 1);
    
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

            if (e.PropertyName?.Equals(nameof(Song.Path)) == true)
            {
                if (!_fileRenamer.Rename(song)) return;
            }

            var isTracked = _dbContext.Songs.Entry(song).State != EntityState.Detached;
            if (isTracked)
            {
                await _dbContext.SaveChangesAsync();
            }

            await _metadataWriter.UpdateTags(song);
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
        Logger.Debug("Tag Editor: Songs collection changed");
        
        // This is the branched out case for the Reset event after the collection is cleared.
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var song in _subscribedSongs)
                song.PropertyChanged -= Song_PropertyChanged;
            _subscribedSongs.Clear();

            return;
        }
        
        if (e.OldItems is not null)
        {
            Logger.Debug("Tag Editor: Old items count: {0}", e.OldItems.Count);
            foreach (Song song in e.OldItems)
            {
                song.PropertyChanged -= Song_PropertyChanged;
                _subscribedSongs.Remove(song);
            }
        }
        
        if (e.NewItems is not null)
        {
            Logger.Debug("Tag Editor: New items count: {0}", e.NewItems.Count);
            foreach (Song song in e.NewItems)
            {
                song.PropertyChanged += Song_PropertyChanged;
                _subscribedSongs.Add(song);
            }
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