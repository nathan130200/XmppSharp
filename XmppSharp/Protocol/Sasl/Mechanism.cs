using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanism", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Mechanism : XElement
{
    public Mechanism() : base(Namespace.Sasl + "mechanism")
    {
        MechanismType = MechanismType.Plain;
    }

    public Mechanism(MechanismType type) : this()
        => MechanismType = type;

    public Mechanism(string mechanismName) : this()
    {
        Require.NotNullOrWhiteSpace(mechanismName);
        MechanismName = mechanismName;
    }

    public MechanismType MechanismType
    {
        get => XmppEnum.ParseOrDefault(Value, MechanismType.Unspecified);
        set
        {
            if (!Enum.IsDefined(value) || value == MechanismType.Unspecified)
                MechanismName = default;
            else
                MechanismName = value.ToXmppName();
        }
    }

    public string? MechanismName
    {
        get => Value;
        set => Value = value ?? string.Empty;
    }
}
