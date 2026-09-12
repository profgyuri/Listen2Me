using Listen2Me.MVVM.Settings.Appearance;
using Listen2Me.MVVM.Settings.Library;
using Listen2Me.MVVM.Settings.Storage;
using Listen2Me.MVVM.Settings.TagEditor;

namespace Listen2Me.MVVM.Settings;

/// <inheritdoc cref="ISettings"/>
public class Settings : ISettings
{
    public Settings(AppearanceSettings appearance, StorageSettings storage, LibrarySettings library, 
        TagEditorSettings tagEditor)
    {
        Appearance = appearance;
        Storage = storage;
        Library = library;
        TagEditor = tagEditor;
    }

    /// <inheritdoc/>
    public AppearanceSettings Appearance { get; set; }
    
    /// <inheritdoc/>
    public StorageSettings Storage { get; set; }
    
    /// <inheritdoc/>
    public LibrarySettings Library { get; set; }
    
    /// <inheritdoc/>
    public TagEditorSettings TagEditor { get; set; }

    /// <inheritdoc/>
    public async Task SaveAsync(CancellationToken ct = default)
    {
        await Task.WhenAll(
            Appearance.SaveAsync(ct), 
            Storage.SaveAsync(ct),
            Library.SaveAsync(ct),
            TagEditor.SaveAsync(ct));
    }
    
    /// <inheritdoc/>
    public async Task LoadAsync(CancellationToken ct = default)
    {
        await Task.WhenAll(
            Appearance.LoadAsync(ct), 
            Storage.LoadAsync(ct),
            Library.LoadAsync(ct),
            TagEditor.LoadAsync(ct));
    }
}