using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Tls;

[XmppTag("proceed", Namespaces.Tls)]
public class Proceed : XmppElement
{
    public Proceed() : base("proceed", Namespaces.Tls)
    {

    }
}
