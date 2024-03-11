using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("abort", Namespace.Sasl)]
public class Abort : Element
{
    public Abort() : base("abort", Namespace.Sasl)
    {

    }
}