using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Tls;

[XmppTag("proceed", Namespace.Tls)]
public class Proceed : Element
{
    public Proceed() : base("proceed", Namespace.Tls)
    {

    }
}
