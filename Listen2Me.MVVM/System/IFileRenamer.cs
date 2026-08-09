using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.System;

/// <summary>
/// Provides a way to rename files.
/// </summary>
public interface IFileRenamer
{
    /// <summary>
    /// Renames the file associated with the song.
    /// </summary>
    /// <param name="song">The song to rename.</param>
    /// <returns>Whether the renaming was successful</returns>
    bool Rename(Song song);
}