namespace Listen2Me.MVVM.Persistence;

/// <summary>
/// Defines a shared database context.
/// </summary>
public interface ISharedDbContext
{
    /// <summary>
    /// Saves all changes made in this context to the underlying databases.
    /// </summary>
    /// <returns>Number of entities saved.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}