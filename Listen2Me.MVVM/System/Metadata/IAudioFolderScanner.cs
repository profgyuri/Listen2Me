using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.System.Metadata;

/// <summary>
/// Provides metadata reading capabilities.
/// </summary>
public interface IAudioFolderScanner
{
    /// <summary>
    /// Scans all music folders for metadata.
    /// </summary>
    /// <param name="progressReporter">Progress reporter reporting the percentage as an integer.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScanFoldersAsync(IProgress<int> progressReporter, CancellationToken ct);
    
    /// <summary>
    /// Scans a single folder for metadata. Does not save to the database.
    /// </summary>
    /// <param name="path">The path to the folder.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The list of songs found in the folder.</returns>
    Task<List<Song>> ScanFolderAsync(string path, CancellationToken ct = default);
}