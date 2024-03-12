using XmppSharp.Attributes;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.StreamFeatures;

[XmppTag("mechanism", Namespace.Sasl)]
public class Mechanism : Element
{
    public Mechanism() : base("mechanism", Namespace.Sasl)
    {

    }

    public Mechanism(MechanismType type) : this()
        => MechanismType = type;

    public Mechanism(string name) : this()
        => MechanismName = name;

    public MechanismType MechanismType
    {
        get => XmppEnum.FromXml(Value, MechanismType.Unspecified);
        set
        {
            MechanismName = value != MechanismType.Unspecified
                ? XmppEnum.ToXml(value) : null;
        }
    }

    public string? MechanismName
    {
        get => Value;
        set => Value = value;
    }
}
