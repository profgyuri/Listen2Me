namespace Listen2Me.MVVM.Persistence.Entities;

public class Bookmark : MusicFolder
{
    /// <summary>
    /// The display name of the bookmark, usually the name of the innermost folder.
    /// </summary>
    public string DisplayName { get; set; }
}