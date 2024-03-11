using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("challenge", Namespace.Sasl)]
public sealed class Challenge : Element
{
    public Challenge() : base("challenge", Namespace.Sasl)
    {

    }
}
