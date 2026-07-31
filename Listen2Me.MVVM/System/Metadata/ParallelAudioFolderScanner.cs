using System.Collections.Concurrent;
using System.IO;
using Listen2Me.MVVM.Persistence;
using Listen2Me.MVVM.Persistence.Entities;
using Serilog;

namespace Listen2Me.MVVM.System.Metadata;

/// <inheritdoc/>
public class ParallelAudioFolderScanner : IAudioFolderScanner
{
    private readonly IMetadataReader _metadataReader;
    private readonly ILogger _logger;
    private readonly ISharedDbContext _dbContext;

    public ParallelAudioFolderScanner(IMetadataReader metadataReader, ILogger logger, ISharedDbContext dbContext)
    {
        _metadataReader = metadataReader;
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task ScanFolderAsync(string path, IProgress<int> progressReporter, CancellationToken ct)
    {
        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
            .Where(IsSupportedAudioExtension)
            .ToList();
        
        var results = new ConcurrentBag<Song>();
        var processed = 0;
        var total = files.Count;

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4,
            CancellationToken = ct
        };
        
        await Parallel.ForEachAsync(files, options, (filePath, token) =>
        {
            try
            {
                var metadata = _metadataReader.Read(filePath);
                results.Add(metadata);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to read metadata for file {FilePath}", filePath);
            }
            finally
            {
                var count = Interlocked.Increment(ref processed);
                if (count % 25 == 0 || count == total)
                    progressReporter?.Report((int)(count / (double)total * 100));
            }
            
            return ValueTask.CompletedTask;
        });
        
        _dbContext.Songs.AddRange(results);
        await _dbContext.SaveChangesAsync(ct);
    }

    private bool IsSupportedAudioExtension(string path)
    {
        return Constants.SupportedAudioExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}