using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanism", Namespaces.Sasl)]
public class Mechanism : Element
{
    public Mechanism() : base("mechanism", Namespaces.Sasl)
    {

    }

    public Mechanism(MechanismType type) : base("mechanism", Namespaces.Sasl)
    {
        Type = type;
    }

    public MechanismType? Type
    {
        get => XmppEnum.FromXml<MechanismType>(Value);
        set
        {
            if (value.TryUnwrap(out var self))
                Value = XmppEnum.ToXml(self);
            else
                Value = null;
        }
    }
}
