using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.Settings.Library;

/// <summary>
/// Holds settings related to the music library.
/// </summary>
public class LibrarySettings : JsonSettingsMemory
{
    public IEnumerable<MusicFolder> MusicFolders { get; set; } = new List<MusicFolder>();
    
    protected override string FileName => "library.json";
}