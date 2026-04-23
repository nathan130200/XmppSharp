using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

/// <summary>
/// Defines the TLS policy for the server, indicating whether TLS is optional or required.
/// </summary>
public enum StartTlsPolicy
{
    /// <summary>
    /// Indicates that the server supports TLS, but it is not mandatory for clients to use it. Clients can choose to proceed without using TLS if they prefer.
    /// </summary>
    [XmppEnumMember("optional")]
    Optional = 1,

    /// <summary>
    /// Indicates that the server requires TLS, and clients must use it to establish a secure connection.
    /// </summary>
    [XmppEnumMember("required")]
    Required = 2,
}