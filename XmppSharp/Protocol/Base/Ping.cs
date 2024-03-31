using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents a "ping" element used within XMPP for checking server aliveness.
/// <para>Sending a ping element to the server and receiving a corresponding "pong" response indicates that the server is reachable.</para>
/// </summary>
[XmppTag("ping", Namespaces.Ping)]
public class Ping : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Ping"/> class.
    /// </summary>
    public Ping() : base("ping", Namespaces.Ping)
    {

    }
}
