using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanism", Namespaces.Sasl)]
public class Mechanism : Element
{
    public Mechanism() : base("mechanism", Namespaces.Sasl)
    {

    }

    public Mechanism(MechanismType type) : this()
        => MechanismType = type;

    public Mechanism(string name) : this()
        => MechanismName = name;

    public MechanismType MechanismType
    {
        get => XmppEnum.ParseOrThrow<MechanismType>(Value);
        set
        {
            if (!Enum.IsDefined(value))
                MechanismName = default;
            else
                MechanismName = value.ToXmppName();
        }
    }

    public string? MechanismName
    {
        get => Value;
        set => Value = value;
    }
}
