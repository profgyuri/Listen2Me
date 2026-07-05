using Listen2Me.MVVM.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listen2Me.MVVM.Persistence.Syncing;

public class SyncService : ISyncService
{
    private readonly SqLiteDataContext _sqlite;
    private readonly PostgresContextFactory _postgresFactory;

    public SyncService(SqLiteDataContext sqlite, PostgresContextFactory postgresFactory)
    {
        _sqlite = sqlite;
        _postgresFactory = postgresFactory;
    }

    // <inheritdoc/>
    public async Task<SyncPreview> BuildSyncPreviewAsync(CancellationToken ct = default)
    {
        await using var postgres = _postgresFactory.Create();

        var upserts = new List<PendingUpsert>();
        var deletes = new List<PendingDelete>();

        await DiffSongsAsync(postgres, upserts, deletes, ct);

        return new SyncPreview { Upserts = upserts, Deletes = deletes };
    }

    // <inheritdoc/>
    public async Task PushAsync(SyncPreview preview, CancellationToken ct = default)
    {
        await using var postgres = _postgresFactory.Create();

        var songUpsertIds = preview.Upserts
            .Where(u => u.EntityName == nameof(Song))
            .Select(u => u.Id)
            .ToList();

        if (songUpsertIds.Count > 0)
        {
            var freshSongs = await _sqlite.Songs
                .AsNoTracking()
                .Where(s => songUpsertIds.Contains(s.Id))
                .ToListAsync(ct);

            var existingIds = await postgres.Songs
                .Where(s => songUpsertIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToHashSetAsync(ct);

            foreach (var song in freshSongs)
            {
                var state = existingIds.Contains(song.Id)
                    ? EntityState.Modified
                    : EntityState.Added;

                postgres.Entry(song).State = state;
            }
        }

        var songDeleteIds = preview.Deletes
            .Where(d => d.EntityName == nameof(Song))
            .Select(d => d.Id)
            .ToList();

        if (songDeleteIds.Count > 0)
        {
            await postgres.Songs
                .Where(s => songDeleteIds.Contains(s.Id))
                .ExecuteDeleteAsync(ct);
        }

        await postgres.SaveChangesAsync(ct);
    }
    
    private async Task DiffSongsAsync(
        PostgresDataContext postgres,
        List<PendingUpsert> upserts,
        List<PendingDelete> deletes,
        CancellationToken ct)
    {
        var sqliteSongs = await _sqlite.Songs
            .AsNoTracking()
            .Select(s => new { s.Id, s.LastWrite })
            .ToDictionaryAsync(s => s.Id, ct);

        var postgresSongs = await postgres.Songs
            .AsNoTracking()
            .Select(s => new { s.Id, s.LastWrite })
            .ToDictionaryAsync(s => s.Id, ct);

        foreach (var (id, sqliteRow) in sqliteSongs)
        {
            if (!postgresSongs.TryGetValue(id, out var postgresRow) ||
                sqliteRow.LastWrite > postgresRow.LastWrite)
            {
                upserts.Add(new PendingUpsert(nameof(Song), id, sqliteRow.LastWrite));
            }
        }

        foreach (var id in postgresSongs.Keys)
        {
            if (!sqliteSongs.ContainsKey(id))
            {
                deletes.Add(new PendingDelete(nameof(Song), id));
            }
        }
    }
}