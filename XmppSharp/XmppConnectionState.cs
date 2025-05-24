namespace XmppSharp;

/// <summary>
/// Represents the various states of an XMPP connection during its lifecycle.
/// </summary>
public enum XmppConnectionState : byte
{
    /// <summary>
    /// Represents the state of being disconnected from a network, service, or resource.
    /// </summary>
    Disconnected,

    /// <summary>
    /// Represents the state of a connection that is in the process of being established.
    /// </summary>
    Connecting,

    /// <summary>
    /// Represents the state of a socket when it is open and actively connected.
    /// </summary>
    Connected,

    /// <summary>
    /// Represents the state of a TCP stream when it is secure and encrypted.
    /// </summary>
    Encrypted,

    /// <summary>
    /// Indicates whether the XMPP connection is authenticated.
    /// </summary>
    Authenticated,

    /// <summary>
    /// Represents the state where a custom resource is bound to the client JID.
    /// </summary>
    ResourceBinded,

    /// <summary>
    /// Indicates that the session has started and the connection is fully ready for communication.
    /// </summary>
    SessionStarted,

    /// <summary>
    /// Represents the state of a disconnection process that is currently in progress.
    /// </summary>
    Disconnecting
}
