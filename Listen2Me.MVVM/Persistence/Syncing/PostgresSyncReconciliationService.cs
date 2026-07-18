using Listen2Me.MVVM.Settings.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;

namespace Listen2Me.MVVM.Persistence.Syncing;

/// <summary>
/// Attempts to synchronize the SqLite database with the PostgreSQL database every 5 minutes.
/// </summary>
public class PostgresSyncReconciliationService: IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
    private Timer? _timer;

    public PostgresSyncReconciliationService(IServiceScopeFactory scopeFactory, ILogger logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(
            callback: _ => FireAndForget(),
            state: null,
            dueTime: TimeSpan.FromSeconds(30),
            period: _interval);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }

    private void FireAndForget()
    {
        _ = ReconcileAsync(CancellationToken.None);
    }
    
    private async Task ReconcileAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();
        var storageSettings = scope.ServiceProvider.GetRequiredService<StorageSettings>();
        
        if (!storageSettings.PostgresStorage.UsePostgres) return;

        try
        {
            var preview = await syncService.BuildSyncPreviewAsync(ct);

            if (preview.Upserts.Count == 0 && preview.Deletes.Count == 0)
            {
                return;
            }

            _logger.Information("Reconciliation found {Upserts} upserts, {Deletes} deletes to push",
                preview.Upserts.Count, preview.Deletes.Count);

            await syncService.PushAsync(preview, ct);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Reconciliation pass failed, will retry next tick");
        }
    }
}