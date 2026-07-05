namespace Listen2Me.MVVM.Persistence.Syncing;

public record PendingUpsert(string EntityName, Guid Id, DateTime LastWrite);