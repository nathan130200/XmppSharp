namespace XmppSharp.Net;

[Flags]
public enum XmppConnectionState
{
    Disconnected,
    Connected = 1 << 0,
    Encrypted = 1 << 1,
    Authenticated = 1 << 2,
    ResourceBinded = 1 << 3,
    SessionStarted = 1 << 4,
}