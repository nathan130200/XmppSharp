using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

/// <summary>
/// The session element is used by the server to indicate that a session has been established.
/// </summary>
[Tag("session", Namespaces.Session)]
public class Session : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class.
    /// </summary>
    public Session() : base("session", Namespaces.Session)
    {

    }
}
