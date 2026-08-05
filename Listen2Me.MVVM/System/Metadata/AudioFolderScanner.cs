using System.Collections.Concurrent;
using System.IO;
using Listen2Me.MVVM.Persistence;
using Listen2Me.MVVM.Persistence.DataContextHelpers;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings;
using Serilog;

namespace Listen2Me.MVVM.System.Metadata;

/// <inheritdoc/>
public class AudioFolderScanner : IAudioFolderScanner
{
    private readonly IMetadataReader _metadataReader;
    private readonly ILogger _logger;
    private readonly ISharedDbContext _dbContext;
    private readonly ISettings _settings;
    private readonly IScanResultProcessor _scanResultProcessor;

    public AudioFolderScanner(IMetadataReader metadataReader, ILogger logger, ISharedDbContext dbContext, 
        ISettings settings, IScanResultProcessor scanResultProcessor)
    {
        _metadataReader = metadataReader;
        _logger = logger;
        _dbContext = dbContext;
        _settings = settings;
        _scanResultProcessor = scanResultProcessor;
    }

    /// <inheritdoc/>
    public async Task ScanFoldersAsync(IProgress<int> progressReporter, CancellationToken ct)
    {
        var folders = _settings.Library.MusicFolders.Select(x => x.Path).ToList();
        var allFiles = new List<string>();
        foreach (var folder in folders)
        {
            if (!Directory.Exists(folder)) continue;
            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(IsSupportedAudioExtension)
                .ToList();
            allFiles.AddRange(files);
        }
        
        if (allFiles.Count == 0) return;
        
        var results = new List<Song>(allFiles.Count);
        var processed = 0;
        var total = allFiles.Count;

        await Task.Run(() =>
        {
            foreach (var path in allFiles)
            {
                ct.ThrowIfCancellationRequested();
                
                try
                {
                    var metadata = _metadataReader.Read(path);
                    results.Add(metadata);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Failed to read metadata for file {FilePath}", path);
                }
                finally
                {
                    processed++;
                    if (processed % 25 == 0 || processed == total)
                        progressReporter?.Report((int)(processed / (double)total * 100));
                }
            }
        }, ct);
        
        await _scanResultProcessor.AddOrUpdateResults(results, ct);
        await _scanResultProcessor.RemoveMissingResults(results, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<List<Song>> ScanFolderAsync(string path, CancellationToken ct = default)
    {
        if (!Directory.Exists(path)) return [];
        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
            .Where(IsSupportedAudioExtension)
            .ToList();

        if (files.Count == 0) return [];
        
        var results = new List<Song>(files.Count);

        await Task.Run(() =>
        {
            foreach (var file in files)
            {
                ct.ThrowIfCancellationRequested();
                
                try
                {
                    var metadata = _metadataReader.Read(file);
                    results.Add(metadata);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Failed to read metadata for file {FilePath}", file);
                }
            }
        }, ct);
        
        return results;
    }

    private bool IsSupportedAudioExtension(string path)
    {
        return Constants.SupportedAudioExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}