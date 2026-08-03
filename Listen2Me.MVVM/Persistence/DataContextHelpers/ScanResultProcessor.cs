using Listen2Me.MVVM.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listen2Me.MVVM.Persistence.DataContextHelpers;

/// <inheritdoc/>
public class ScanResultProcessor : IScanResultProcessor
{
    private readonly ISharedDbContext _dbContext;

    public ScanResultProcessor(ISharedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateResults(List<Song> songs, CancellationToken ct = default)
    {
        var scannedPaths = songs.Select(s => s.Path).ToList();

        var existingByPath = await _dbContext.Songs
            .Where(x => scannedPaths.Contains(x.Path))
            .ToDictionaryAsync(x => x.Path, ct);

        var toAdd = new List<Song>();
        foreach (var song in songs)
        {
            if (existingByPath.TryGetValue(song.Path, out var existing))
                existing.MapFrom(song);
            else
                toAdd.Add(song);
        }

        _dbContext.Songs.AddRange(toAdd);
    }

    /// <inheritdoc/>
    public async Task RemoveMissingResults(List<Song> songs, CancellationToken ct = default)
    {
        var savedSongs = await _dbContext.Songs.ToListAsync(ct);
        var scannedPaths = songs.Select(s => s.Path).ToList();
        var toRemove = savedSongs.Where(s => !scannedPaths.Contains(s.Path)).ToList();
        _dbContext.Songs.RemoveRange(toRemove);
    }
}