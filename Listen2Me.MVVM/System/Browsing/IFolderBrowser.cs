namespace Listen2Me.MVVM.System.Browsing;

/// <summary>
/// Provides a simple folder browser.
/// </summary>
public interface IFolderBrowser
{
    /// <summary>
    /// The folder currently being displayed/navigated.
    /// </summary>
    string CurrentPath { get; set; }
    
    /// <summary>
    /// Gets the top-level entry points (drives).
    /// </summary>
    IReadOnlyList<string> GetRoots();
    
    /// <summary>
    /// Enumerates the subfolders of the current (or a given) path.
    /// </summary>
    Task<IReadOnlyList<string>> GetSubFolders(string? path = null);
    
    /// <summary>
    /// Navigates directly into a folder.
    /// </summary>
    void NavigateTo(string path);
    
    /// <summary>
    /// Navigates to the parent of the current folder, if any.
    /// </summary>
    void NavigateUp();

    /// <summary>
    /// Navigates to a child folder of the current folder.
    /// </summary>
    /// <param name="name">Name of the child folder.</param>
    void NavigateToChild(string name);
}