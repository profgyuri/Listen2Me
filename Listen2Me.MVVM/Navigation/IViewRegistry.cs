using Listen2Me.MVVM.ViewModels;

namespace Listen2Me.MVVM.Navigation;

/// <summary>
/// Stores shell - view model pairs.
/// </summary>
public interface IViewRegistry
{
    /// <summary>
    /// Registers a view - view model pair.
    /// </summary>
    void Register<TViewModel, TView>() where TViewModel : class where TView : class;

    /// <summary>
    /// Resolves a view from a view model type.
    /// </summary>
    Type Resolve<TViewModel>() where TViewModel : class;
}