namespace Listen2Me.WPF.Modules.Example;

/// <summary>
/// Provides sample status text for the example module.
/// </summary>
public interface IExampleStatusService
{
    /// <summary>
    /// Gets the current status text asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>The status text.</returns>
    Task<string> GetStatusAsync(CancellationToken cancellationToken = default);
}