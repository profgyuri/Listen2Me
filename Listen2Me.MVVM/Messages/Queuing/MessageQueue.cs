namespace Listen2Me.MVVM.Messages.Queuing;

/// <inheritdoc/>
public class MessageQueue : IMessageQueue
{
    private readonly List<object> _bag = new();
    
    /// <inheritdoc/>
    public void Enqueue<TMessage>(TMessage message) where TMessage : class
    {
        _bag.Add(message);
    }

    /// <inheritdoc/>
    public TMessage? Dequeue<TMessage>() where TMessage : class
    {
        var message = _bag.FirstOrDefault(x => x is TMessage);
        if (message is not null) _bag.Remove(message);
        return (TMessage?)message;
    }

    internal List<object> GetQueue()
    {
        return _bag;
    }
}