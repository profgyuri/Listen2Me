using Listen2Me.MVVM.ViewModels;

namespace Listen2Me.MVVM.Navigation;

/// <summary>
/// Stores shell - view model pairs.
/// </summary>
public interface IShellRegistry
{
    /// <summary>
    /// Registers a shell - view model pair.
    /// </summary>
    /// <typeparam name="TShellViewModel">The shell view model type.</typeparam>
    /// <typeparam name="TShell">The shell window type.</typeparam>
    void Register<TShellViewModel, TShell>() where TShellViewModel : ShellViewModelBase where TShell : class;
    
    /// <summary>
    /// Resolves a shell from a view model type.
    /// </summary>
    /// <typeparam name="TShellViewModel">The view model used to resolve the shell.</typeparam>
    /// <returns></returns>
    Type Resolve<TShellViewModel>() where TShellViewModel : ShellViewModelBase;
}