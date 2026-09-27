using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.System.Metadata;

/// <summary>
/// Provides methods for updating metadata.
/// </summary>
public interface IMetadataWriter
{
    /// <summary>
    /// Updates tags for an audio file.
    /// </summary>
    /// <param name="song">The song containing the new metadata.</param>
    Task UpdateTags(Song song);
}