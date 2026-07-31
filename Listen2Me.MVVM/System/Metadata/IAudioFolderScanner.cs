using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.System.Metadata;

/// <summary>
/// Provides metadata reading capabilities.
/// </summary>
public interface IAudioFolderScanner
{
    /// <summary>
    /// Scans a folder for metadata.
    /// </summary>
    /// <param name="path">The folder that contains supported media files.</param>
    /// <param name="progressReporter">Progress reporter reporting the percentage as an integer.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScanFolderAsync(string path, IProgress<int> progressReporter, CancellationToken ct);
}