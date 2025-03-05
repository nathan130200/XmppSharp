using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanism", Namespaces.Sasl)]
public class Mechanism : XmppElement
{
    public Mechanism() : base("mechanism", Namespaces.Sasl)
    {

    }

    public Mechanism(string mechanismName) : this()
    {
        Throw.IfStringNullOrWhiteSpace(mechanismName);
        Value = mechanismName;
    }
}
