using System.Windows;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Listen2Me.WPF.Navigation;

/// <inheritdoc cref="IDialogManager"/>
public class DialogManager : IDialogManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IViewRegistry _viewRegistry;
    private readonly ILogger _logger;

    public DialogManager(IServiceProvider serviceProvider, IViewRegistry viewRegistry, ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _viewRegistry = viewRegistry;
        _logger = logger;
    }

    public async Task<TResult?> ShowDialogAsync<TDialogViewModel, TResult>(CancellationToken ct = default) 
        where TDialogViewModel : DialogViewModelBase<TResult>
    {
        var vm = _serviceProvider.GetRequiredService<TDialogViewModel>();
        var view = _viewRegistry.Resolve<TDialogViewModel>();
        _logger.Debug("Resolved dialog vm: {Id}", vm.Id);

        var window = (Window)ActivatorUtilities.CreateInstance(_serviceProvider, view, vm);
        window.Owner = Application.Current.MainWindow;

        await vm.EnsureInitializedAsync(ct);
        await vm.OnOpenAsync(ct);

        var dialogResult = window.ShowDialog();
        return dialogResult == true ? vm.Result : default;
    }
}