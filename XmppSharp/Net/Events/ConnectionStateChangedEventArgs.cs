namespace XmppSharp.Net.Abstractions;

public sealed class ConnectionStateChangedEventArgs
{
    public XmppConnection Connection { get; init; }
    public XmppConnectionState OldState { get; init; }
    public XmppConnectionState NewState { get; init; }
}
