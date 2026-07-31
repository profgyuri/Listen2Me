using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.System.Metadata;

/// <summary>
/// Provides metadata reading capabilities.
/// </summary>
public interface IMetadataReader
{
    /// <summary>
    /// Reads metadata from a file.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>Instance of <see cref="Song"/>.</returns>
    Song Read(string path);
}