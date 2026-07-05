namespace Listen2Me.MVVM.Persistence.Syncing;

public interface ISyncableEntity
{
    Guid Id { get; }
    DateTime LastWrite { get; }
}