namespace Listen2Me.MVVM.Persistence.Syncing;

public record SyncPreview
{
    public required IReadOnlyList<PendingUpsert> Upserts { get; init; }
    public required IReadOnlyList<PendingDelete> Deletes { get; init; }
}