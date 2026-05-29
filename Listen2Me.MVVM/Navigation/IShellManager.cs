using Listen2Me.MVVM.ViewModels;

namespace Listen2Me.MVVM.Navigation;

public interface IShellManager
{
    /// <summary>
    /// Opens the shell assigned to the specified view model.
    /// </summary>
    /// <param name="ct">Token to cancel the operation.</param>
    /// <typeparam name="TShellViewModel">Type of the shell view model.</typeparam>
    Task OpenShell<TShellViewModel>(CancellationToken ct) where TShellViewModel : ShellViewModelBase;
}