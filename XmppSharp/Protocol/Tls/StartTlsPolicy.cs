using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

/// <summary>
/// Enumerates the possible TLS (Transport Layer Security) policies used in the "starttls" element during XMPP communication.
/// </summary>
[XmppEnum]
public enum StartTlsPolicy
{
    /// <summary>
    /// Indicates that TLS is optional for the connection.
    /// </summary>
    [XmppMember("optional")]
    Optional,

    /// <summary>
    /// Indicates that TLS is required for the connection.
    /// </summary>
    [XmppMember("required")]
    Required
}