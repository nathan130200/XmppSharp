using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("success", Namespace.Sasl)]
public sealed class Success : Element
{
    public Success() : base("success", Namespace.Sasl)
    {

    }
}
