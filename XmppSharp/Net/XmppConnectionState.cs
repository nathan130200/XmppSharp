namespace XmppSharp.Net;

public enum XmppConnectionState : byte
{
    Disconnected,
    Connected,
    Encrypted,
    Authenticated,
    ResourceBinded,
    SessionStarted,
    Disconnecting
}
