using System.Collections.ObjectModel;
using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.Controllers;

/// <summary>
/// Controller for drag and drop operations.
/// </summary>
public interface IDragNDropController
{
    /// <summary>
    /// If the dropped paths are already in the db, it will be added from there,
    /// otherwise it will be scanned without db access.
    /// </summary>
    /// <param name="songs">The collection to add the songs to.</param>
    /// <param name="paths">Either the paths of the files or the paths to the containing folders.</param>
    Task AddDroppedPathsToCollection(ObservableCollection<Song> songs, string[] paths);
}