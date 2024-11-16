using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Sasl;

[XmppTag("mechanism", Namespaces.Sasl)]
public class Mechanism : Element
{
    public Mechanism() : base("mechanism", Namespaces.Sasl)
    {

    }

    public Mechanism(string mechanismName) : this()
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(mechanismName);
        Value = mechanismName;
    }
}
