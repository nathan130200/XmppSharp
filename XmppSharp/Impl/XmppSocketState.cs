namespace XmppSharp;

/// <summary>
/// Set of flags that define the state of the XMPP socket for I/O operations..
/// </summary>
[Flags]
public enum XmppSocketState
{
    /// <summary>
    /// Socket is temporarily unavailable.
    /// </summary>
    None = 0,

    /// <summary>
    /// Socket has read permission.
    /// </summary>
    Readable = 1 << 0,

    /// <summary>
    /// Socket has write permission.
    /// </summary>
    Writable = 1 << 1,

    /// <summary>
    /// Socket has started cleanup process.
    /// <para>
    /// At this point the socket stops reading permanently while it tries to send the last packets pending in the queue.
    /// </para>
    /// </summary>
    Disposing = 1 << 2,

    /// <summary>
    /// Socket cleanup process completed.
    /// <para>
    /// At this point the socket considers that all pending packets in the queue have been delivered and the connection is closed.
    /// </para>
    /// </summary>
    Disposed = 1 << 3
}
