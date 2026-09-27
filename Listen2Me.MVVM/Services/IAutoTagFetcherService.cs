using System.Collections.ObjectModel;
using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.Services;

/// <summary>
/// Provides methods to get tags from filename or vice versa
/// </summary>
public interface IAutoTagFetcherService
{
    /// <summary>
    /// Calls the TagEditorFormula dialog and reads the tags from the filename
    /// </summary>
    /// <param name="songs">These songs' tag metadata will be updated based on their paths</param>
    Task FetchTagsFromFilenameAsync(ObservableCollection<Song> songs);
    
    /// <summary>
    /// Calls the TagEditorFormula dialog and renames the files using their tags.
    /// </summary>
    /// <param name="songs">These songs' filename will be updated based on their tags</param>
    Task FetchFilenameFromTagsAsync(ObservableCollection<Song> songs);
}