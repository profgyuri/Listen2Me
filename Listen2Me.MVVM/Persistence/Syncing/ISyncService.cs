namespace Listen2Me.MVVM.Persistence.Syncing;

public interface ISyncService
{
    /// <summary>
    /// Provides a preview of the changes that will be pushed to the postgres server.
    /// </summary>
    /// <returns>A list of changes to be pushed.</returns>
    Task<SyncPreview> BuildSyncPreviewAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Pushes the changes to the postgres server.
    /// </summary>
    /// <param name="preview">The preview of the changes to be pushed. Provided by <see cref="BuildSyncPreviewAsync"/>.</param>
    Task PushAsync(SyncPreview preview, CancellationToken ct = default);
}