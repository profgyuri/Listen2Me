using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings;
using Microsoft.EntityFrameworkCore;

namespace Listen2Me.MVVM.Persistence;

public sealed class SharedDataContext : ISharedDbContext
{
    private readonly SqLiteDataContext _sqLiteDataContext;
    private readonly PostgresContextFactory _postgresDataContextFactory;
    private readonly ISettings _settings;

    public SharedDataContext(SqLiteDataContext sqLiteDataContext, PostgresContextFactory postgresDataContextFactory, 
        ISettings settings)
    {
        _sqLiteDataContext = sqLiteDataContext;
        _postgresDataContextFactory = postgresDataContextFactory;
        _settings = settings;
    }

    #region DbSets
    public DbSet<Song> Songs => _sqLiteDataContext.Songs;
    #endregion
    
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // Save to postgres only, if explicitly set so
        await using var postgresContext = _settings.Storage.PostgresStorage.UsePostgres
            ? _postgresDataContextFactory.Create()
            : null;

        if (postgresContext is not null) // If postgres saving is set, copy changes from sqlite to postgres
        {
            foreach (var entry in _sqLiteDataContext.ChangeTracker.Entries())
            {
                postgresContext.Entry(entry.Entity).State = entry.State;
            }
        }

        // SaveChanges sets the state to Unchanged, so copying has to happen beforehand
        var result = await _sqLiteDataContext.SaveChangesAsync(ct);

        if (postgresContext is not null)
        {
            await postgresContext.SaveChangesAsync(ct);
        }

        return result;
    }
}