namespace XmppSharp.Abstractions;

public sealed class XmppLoggingEventArgs
{
    public object Sender { get; init; }
    public DateTime Timestamp { get; init; }
    public XmppLogLevel Level { get; init; }
    public string? Message { get; init; }
    public Exception? Exception { get; init; }
}
