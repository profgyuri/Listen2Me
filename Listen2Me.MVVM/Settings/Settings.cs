using Listen2Me.MVVM.Settings.Appearance;
using Listen2Me.MVVM.Settings.Library;
using Listen2Me.MVVM.Settings.Storage;

namespace Listen2Me.MVVM.Settings;

/// <inheritdoc cref="ISettings"/>
public class Settings : ISettings
{
    public Settings(AppearanceSettings appearance, StorageSettings storage, LibrarySettings library)
    {
        Appearance = appearance;
        Storage = storage;
        Library = library;
    }

    /// <inheritdoc/>
    public AppearanceSettings Appearance { get; set; }
    
    /// <inheritdoc/>
    public StorageSettings Storage { get; set; }
    
    /// <inheritdoc/>
    public LibrarySettings Library { get; set; }

    /// <inheritdoc/>
    public async Task SaveAsync(CancellationToken ct = default)
    {
        await Task.WhenAll(
            Appearance.SaveAsync(ct), 
            Storage.SaveAsync(ct),
            Library.SaveAsync(ct));
    }
    
    /// <inheritdoc/>
    public async Task LoadAsync(CancellationToken ct = default)
    {
        await Task.WhenAll(
            Appearance.LoadAsync(ct), 
            Storage.LoadAsync(ct),
            Library.LoadAsync(ct));
    }
}