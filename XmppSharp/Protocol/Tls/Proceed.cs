using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

/// <summary>
/// Represents a "proceed" element used in the TLS (Transport Layer Security) negotiation within XMPP. 
/// <para>Receiving a "proceed" element indicates that the server is ready to begin the TLS handshake.</para>
/// <para>In this case, both the sides should initiate the handshake process on their respective sockets.</para>
/// </summary>
[XmppTag("proceed", Namespaces.Tls)]
public sealed class Proceed : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Proceed"/> class.
    /// </summary>
    public Proceed() : base("proceed", Namespaces.Tls)
    {

    }
}
