namespace Listen2Me.MVVM.Messages.Queuing;

/// <summary>
/// Aimed to be used as a message queue, in case we need to send messages between views with mistimed initializations.
/// </summary>
public interface IMessageQueue
{
    /// <summary>
    /// Enqueue a message.
    /// </summary>
    /// <param name="message">The object to send as a message.</param>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    void Enqueue<TMessage>(TMessage message) where TMessage : class;
    
    /// <summary>
    /// Dequeue a message. Always returns the first item with the specified type.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <returns>The message object or null if there is no matching type in the queue.</returns>   
    TMessage? Dequeue<TMessage>() where TMessage : class;
}