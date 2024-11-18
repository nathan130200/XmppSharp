namespace XmppSharp;

/// <summary>
/// Set of flags that define the state of an XMPP connection.
/// </summary>
[Flags]
public enum XmppConnectionState
{
    Disconnected,

    /// <summary>
    /// State defines when the connection is open.
    /// </summary>
    Connected = 1 << 0,

    /// <summary>
    /// State defines when the connection is encrypted.
    /// </summary>
    Encrypted = 1 << 1,

    /// <summary>
    /// State defines when the connection is using compression algorithm.
    /// </summary>
    Compressed = 1 << 2,

    /// <summary>
    /// State defines when the connection is authorized.
    /// </summary>
    Authenticated = 1 << 3,

    /// <summary>
    /// State defines when the connection is bound to a resource.
    /// </summary>
    ResourceBinded = 1 << 4,

    /// <summary>
    /// State defines when the connection is ready.
    /// </summary>
    SessionStarted = 1 << 5
}
