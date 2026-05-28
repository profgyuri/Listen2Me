namespace Listen2Me.MVVM.Threading;

/// <summary>
/// Represents a UI thread dispatcher abstraction for marshaling UI-bound work.
/// </summary>
public interface IUiDispatcher
{
    /// <summary>
    /// Gets a value that indicates whether the current thread has UI access.
    /// </summary>
    bool CheckAccess();

    /// <summary>
    /// Executes an action on the UI thread.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">A token that can cancel the dispatch operation.</param>
    /// <returns>A task representing the dispatch operation.</returns>
    Task InvokeAsync(Action action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a function on the UI thread and returns a result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The function to execute.</param>
    /// <param name="cancellationToken">A token that can cancel the dispatch operation.</param>
    /// <returns>A task that resolves to the function result.</returns>
    Task<T> InvokeAsync<T>(Func<T> action, CancellationToken cancellationToken = default);
}