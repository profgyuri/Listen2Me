namespace Listen2Me.WPF.Modules.Example;

/// <summary>
/// Implements sample asynchronous status retrieval.
/// </summary>
public sealed class ExampleStatusService : IExampleStatusService
{
    /// <inheritdoc />
    public async Task<string> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(120, cancellationToken);
        return $"Example module loaded at {DateTimeOffset.Now:T}";
    }
}