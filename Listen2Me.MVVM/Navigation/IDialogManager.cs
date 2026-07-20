using Listen2Me.MVVM.ViewModels;

namespace Listen2Me.MVVM.Navigation;

/// <summary>
/// Used to handle dialogs.
/// </summary>
public interface IDialogManager
{
    /// <summary>
    /// Shows a dialog.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <typeparam name="TDialogViewModel">ViewModel type of the dialog (data context).</typeparam>
    /// <typeparam name="TResult">Type of the dialog result.</typeparam>
    /// <returns>Task that represents the asynchronous operation.</returns>
    Task<TResult?> ShowDialogAsync<TDialogViewModel, TResult>(CancellationToken ct = default)
        where TDialogViewModel : DialogViewModelBase<TResult>;
}