using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Client;

[XmppTag("session", Namespace.Session)]
public class Session : Element
{
    public Session() : base("session", Namespace.Session)
    {

    }
}