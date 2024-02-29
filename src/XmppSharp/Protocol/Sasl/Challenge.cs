using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("challenge", Namespaces.Sasl)]
public class Challenge : Element
{
    public Challenge() : base("challenge", Namespaces.Sasl)
    {

    }
}
