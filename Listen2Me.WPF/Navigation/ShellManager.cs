using System.Windows;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Navigation;

public class ShellManager : IShellManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IShellRegistry _shellRegistry;

    public ShellManager(IServiceProvider serviceProvider, IShellRegistry shellRegistry)
    {
        _serviceProvider = serviceProvider;
        _shellRegistry = shellRegistry;
    }

    public async Task OpenShell<TShellViewModel>(CancellationToken ct) where TShellViewModel : ShellViewModelBase
    {
        var shellVm = _serviceProvider.GetRequiredService<TShellViewModel>();
        var shell = _shellRegistry.Resolve<TShellViewModel>();
        
        var window = (Window)ActivatorUtilities.CreateInstance(_serviceProvider, shell, shellVm);
        await shellVm.EnsureInitializedAsync(ct);
        window.DataContext = shellVm;
        window.Show();
    }
}