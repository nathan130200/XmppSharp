using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Tls;

[XmppTag("proceed", Namespaces.Tls)]
public sealed class Proceed : Element
{
    public Proceed() : base("proceed", Namespaces.Tls)
    {

    }
}