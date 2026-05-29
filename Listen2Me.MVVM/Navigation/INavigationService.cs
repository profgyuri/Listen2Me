using Listen2Me.MVVM.ViewModels;

namespace Listen2Me.MVVM.Navigation;

/// <summary>
/// Defines asynchronous shell navigation.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to a route within the shell.
    /// </summary>
    /// <param name="route">The route key.</param>
    /// <param name="parameter">An optional navigation parameter.</param>
    /// <param name="cancellationToken">A token that can cancel navigation.</param>
    /// <returns>A task representing the asynchronous navigation operation.</returns>
    Task NavigateToRouteAsync(string route, object? parameter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a value that indicates whether a route is registered.
    /// </summary>
    /// <param name="route">The route key.</param>
    /// <returns><see langword="true"/> if the route exists; otherwise, <see langword="false"/>.</returns>
    bool CanNavigate(string route);
}