using CommunityToolkit.Mvvm.ComponentModel;

namespace Listen2Me.MVVM.Settings.Storage;

public partial class StorageSettings : JsonSettingsMemory
{
    [ObservableProperty] private PostgresStorageSettings _postgresStorage;

    protected override string FileName => "storage.json";
}