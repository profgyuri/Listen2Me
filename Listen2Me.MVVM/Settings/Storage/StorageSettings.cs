namespace Listen2Me.MVVM.Settings.Storage;

public class StorageSettings : IStorageSettings
{
    public PostgresStorageSettings PostgresStorage { get; set; }

    public StorageSettings()
    {
        //todo: load storage settings
    }
}