namespace Listen2Me.MVVM.Initialization;

/// <summary>
/// Defines asynchronous initialization for view models or services.
/// </summary>
public interface IInitializeAsync
{
    /// <summary>
    /// Initializes the instance asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token that can cancel initialization.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}