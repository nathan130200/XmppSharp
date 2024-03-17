using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Base;

[XmppTag("ping", Namespace.Ping)]
public class Ping : Element
{
    public Ping() : base("ping", Namespace.Ping)
    {

    }
}
