using Listen2Me.MVVM.ViewModels;

namespace Listen2Me.MVVM.Navigation;

/// <inheritdoc/>
public class ShellRegistry : IShellRegistry
{
    private readonly Dictionary<Type, Type> _map = new();
    
    /// <inheritdoc/>
    public void Register<TShellViewModel, TShell>() where TShellViewModel : ShellViewModelBase where TShell : class
    {
        if (!_map.TryAdd(typeof(TShellViewModel), typeof(TShell)))
        {
            throw new InvalidOperationException(
                $"{typeof(TShellViewModel).Name} is already mapped to {_map[typeof(TShellViewModel)].Name}.");
        }
    }

    /// <inheritdoc/>
    public Type Resolve<TShellViewModel>() where TShellViewModel : ShellViewModelBase
    {
        if (_map.TryGetValue(typeof(TShellViewModel), out var windowType))
            return windowType;

        throw new InvalidOperationException(
            $"No window registered for shell ViewModel {typeof(TShellViewModel).Name}.");
    }
}