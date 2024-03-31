using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

/// <summary>
/// Represents a "session" element used within XMPP for session establishment.
/// It might be used within specific session negotiation mechanisms within XMPP, depending on the context.
/// </summary>
[XmppTag("session", Namespaces.Session)]
public class Session : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class.
    /// </summary>
    public Session() : base("session", Namespaces.Session)
    {

    }
}