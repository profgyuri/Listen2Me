using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.Persistence.DataContextHelpers;

/// <summary>
/// Provides a way to process scan results.
/// </summary>
public interface IScanResultProcessor
{
    /// <summary>
    /// Adds new songs or updates existing ones. Does not save changes.
    /// </summary>
    /// <param name="songs">The scan results.</param>
    /// <param name="ct">A cancellation token.</param>   
    Task AddOrUpdateResults(List<Song> songs, CancellationToken ct = default);
    
    /// <summary>
    /// Removes songs that are not present in the scan results. Does not save changes.
    /// </summary>
    /// <param name="songs">The scan results.</param>
    /// <param name="ct">A cancellation token.</param> 
    Task RemoveMissingResults(List<Song> songs, CancellationToken ct = default);
}