using System.Collections.ObjectModel;
using System.IO;
using Listen2Me.MVVM.Extensions;
using Listen2Me.MVVM.Persistence;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings;
using Listen2Me.MVVM.System.Metadata;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Listen2Me.MVVM.Controllers;

/// <inheritdoc/>
public class DragNDropController : IDragNDropController
{
    private readonly ISharedDbContext _dbContext;
    private readonly IMetadataReader _metadataReader;
    private readonly ISettings _settings;
    private readonly IAudioFolderScanner _folderScanner;
    private readonly ILogger _logger;

    public DragNDropController(ISharedDbContext dbContext, IMetadataReader metadataReader, ISettings settings, IAudioFolderScanner folderScanner, ILogger logger)
    {
        _dbContext = dbContext;
        _metadataReader = metadataReader;
        _settings = settings;
        _folderScanner = folderScanner;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task AddDroppedPathsToCollection(ObservableCollection<Song> songs, string[] paths)
    {
        foreach (var path in paths)
        {
            try
            {
                var isSupported = Constants.SupportedAudioExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
                if (File.Exists(path) && isSupported)
                {
                    await GetAndAddFromFile(path);
                }
                else if (Directory.Exists(path))
                {
                    await GetAndAddFromFolder(path);
                }
                else
                {
                    _logger.Warning("Dropped path {Path} is not supported", path);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to handle dropped path {Path}", path);
            }
        }
        return;

        async Task GetAndAddFromFile(string path)
        {
            var toAdd = await _dbContext.Songs.FirstOrDefaultAsync(s => s.Path == path);
            toAdd ??= _metadataReader.Read(path);
                    
            if (!songs.Contains(toAdd))
                songs.Add(toAdd);
        }
        
        async Task GetAndAddFromFolder(string path1)
        {
            List<Song> toAdd;
            var existsInDb =
                _settings.Library.MusicFolders.Any(x => x.Path.Equals(path1, StringComparison.OrdinalIgnoreCase));
                    
            if (existsInDb)
            {
                toAdd = await _dbContext.Songs.Where(s => 
                    EF.Functions.Like(s.Path, path1 + "%")).ToListAsync();
            }
            else
            {
                toAdd = await _folderScanner.ScanFolderAsync(path1);
            }

            toAdd = toAdd.Except(songs).ToList();
            songs.AddRange(toAdd);
        }
    }
}