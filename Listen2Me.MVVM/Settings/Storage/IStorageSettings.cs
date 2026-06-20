namespace Listen2Me.MVVM.Settings.Storage;

/// <summary>
/// Entry point for settings related to storage.
/// </summary>
public interface IStorageSettings
{
    /// <inheritdoc cref="PostgresStorageSettings"/>
    public PostgresStorageSettings PostgresStorage { get; set; }
}