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
    private readonly IMetadataWriter _metadataWriter;

    [ObservableProperty] private ICollectionView _songView;
    [ObservableProperty] private ObservableCollection<Song> _songs = new();
    [ObservableProperty] private ObservableCollection<Song> _selectedSongs = new();
    [ObservableProperty] private string _filterText = string.Empty;
    
    public TagEditorLayoutViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        ISharedDbContext dbContext, IDialogManager dialogManager, IAudioFolderScanner folderScanner, 
        IMetadataWriter metadataWriter) 
        : base(errorHandler, logger, messenger)
    {
        _dbContext = dbContext;
        _dialogManager = dialogManager;
        _folderScanner = folderScanner;
        _metadataWriter = metadataWriter;
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

    private void Song_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var song = (Song)sender!;
        Logger.Information("Tag Editor: Song {0} changed", song.Path);
        
        if (_dbContext.Songs.Contains(song))
        {
            _dbContext.Songs.Update(song);
            _dbContext.SaveChangesAsync();
        }
        
        _metadataWriter.UpdateTags(song);
    }

    private void Songs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Logger.Debug("Tag Editor: Songs collection changed");
        if (e.OldItems is not null)
        {
            Logger.Debug("Tag Editor: Old items count: {0}", e.OldItems.Count);
            foreach (Song song in e.OldItems)
            {
                song.PropertyChanged -= Song_PropertyChanged;
            }
        }
        
        if (e.NewItems is not null)
        {
            Logger.Debug("Tag Editor: New items count: {0}", e.NewItems.Count);
            foreach (Song song in e.NewItems)
            {
                song.PropertyChanged += Song_PropertyChanged;
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