using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.Settings.Library;

/// <summary>
/// Holds settings related to the music library.
/// </summary>
public class LibrarySettings : JsonSettingsMemory
{
    /// <summary>
    /// Gets or sets the music folders from the settings file.
    /// </summary>
    public IEnumerable<MusicFolder> MusicFolders { get; set; } = new List<MusicFolder>();

    /// <summary>
    /// Gets or sets the bookmarks from the settings file.
    /// </summary>
    public IEnumerable<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    
    /// <summary>
    /// Gets or sets a value indicating whether the library should be scanned automatically.
    /// </summary>
    public bool ScanAutomatically { get; set; }
    
    protected override string FileName => "library.json";
}