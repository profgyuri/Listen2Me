using System.Collections.ObjectModel;
using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.Services;

/// <summary>
/// Contains methods that help replace files
/// </summary>
public interface IFileMoverService
{
    /// <summary>
    /// Moves multiple song files based on the called folder browser dialog's chosen path
    /// </summary>
    /// <param name="songs">The song to replace</param>
    Task MoveFilesAsync(ObservableCollection<Song> songs);
}