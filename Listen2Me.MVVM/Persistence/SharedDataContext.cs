using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Core;

namespace Listen2Me.MVVM.Persistence;

public sealed class SharedDataContext : ISharedDbContext
{
    private readonly SqLiteDataContext _sqLiteDataContext;
    private readonly PostgresContextFactory _postgresDataContextFactory;
    private readonly ISettings _settings;
    private readonly ILogger _logger;
    
    private readonly AsyncRetryPolicy _postgresRetryPolicy;

    public SharedDataContext(SqLiteDataContext sqLiteDataContext, PostgresContextFactory postgresDataContextFactory, 
        ISettings settings, ILogger logger)
    {
        _sqLiteDataContext = sqLiteDataContext;
        _postgresDataContextFactory = postgresDataContextFactory;
        _settings = settings;
        _logger = logger;

        _postgresRetryPolicy = Policy
            .Handle<NpgsqlException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)),
                onRetry: (ex, _, attempt, _) =>
                {
                    if (ex is not null)
                        _logger.Warning(ex, "Postgres save failed, retry {Attempt}/3", attempt);
                });
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
        _logger.Information("Saved {Count} changes to SQLite.", result);

        if (postgresContext is not null)
        {
            try
            {
                await _postgresRetryPolicy.ExecuteAsync(postgresContext.SaveChangesAsync, ct);
                _logger.Information("Saved {Count} changes to Postgres.", result);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Saving to Postgres failed.");
            }
        }

        return result;
    }
}