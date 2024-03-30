using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

[XmppTag("proceed", Namespaces.Tls)]
public class Proceed : Element
{
    public Proceed() : base("proceed", Namespaces.Tls)
    {

    }
}
