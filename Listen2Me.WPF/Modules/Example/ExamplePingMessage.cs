using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Listen2Me.WPF.Modules.Example;

/// <summary>
/// Represents a sample module message payload.
/// </summary>
public sealed class ExamplePingMessage : ValueChangedMessage<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExamplePingMessage"/> class.
    /// </summary>
    /// <param name="value">The message value.</param>
    public ExamplePingMessage(string value)
        : base(value)
    {
    }
}