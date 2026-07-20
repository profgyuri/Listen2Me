using System.Windows;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Navigation;

/// <inheritdoc cref="IDialogManager"/>
public class DialogManager : IDialogManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IViewRegistry _viewRegistry;

    public DialogManager(IServiceProvider serviceProvider, IViewRegistry viewRegistry)
    {
        _serviceProvider = serviceProvider;
        _viewRegistry = viewRegistry;
    }

    public async Task<TResult?> ShowDialogAsync<TDialogViewModel, TResult>(CancellationToken ct = default) 
        where TDialogViewModel : DialogViewModelBase<TResult>
    {
        var vm = _serviceProvider.GetRequiredService<TDialogViewModel>();
        var view = _viewRegistry.Resolve<TDialogViewModel>();

        var window = (Window)ActivatorUtilities.CreateInstance(_serviceProvider, view, vm);
        window.Owner = Application.Current.MainWindow;

        await vm.EnsureInitializedAsync(ct);

        var dialogResult = window.ShowDialog();
        return dialogResult == true ? vm.Result : default;
    }
}