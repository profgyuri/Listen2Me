using System.Windows.Threading;
using Listen2Me.MVVM.Threading;

namespace Listen2Me.WPF.Threading;

public class WpfUiDispatcher : IUiDispatcher
{
    private readonly Dispatcher _dispatcher;

    public WpfUiDispatcher(Dispatcher dispatcher)
        => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

    public bool CheckAccess() => _dispatcher.CheckAccess();

    public Task InvokeAsync(Action action, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (ct.IsCancellationRequested)
        {
            return Task.FromCanceled(ct);
        }

        if (CheckAccess())
        {
            action();
            return Task.CompletedTask;
        }

        return _dispatcher.InvokeAsync(action, DispatcherPriority.DataBind, ct).Task;
    }

    public Task<T> InvokeAsync<T>(Func<T> func, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(func);

        if (ct.IsCancellationRequested)
        {
            return Task.FromCanceled<T>(ct);
        }

        return CheckAccess()
            ? Task.FromResult(func())
            : _dispatcher.InvokeAsync(func, DispatcherPriority.DataBind, ct).Task;
    }
}