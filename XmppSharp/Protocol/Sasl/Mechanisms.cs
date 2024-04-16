using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanisms", Namespace.Sasl)]
public class Mechanisms : Element
{
    public Mechanisms() : base("mechanisms", Namespace.Sasl)
    {

    }

    public IEnumerable<Mechanism> SupportedMechanisms
    {
        get => Children<Mechanism>();
        set
        {
            Children().Remove();

            foreach (var item in value)
                AddChild(item);
        }
    }

    public void AddMechanism(MechanismType type)
    {
        if (type == MechanismType.Unspecified || !Enum.IsDefined(type))
            return;

        AddMechanism(type.ToXmppName());
    }

    public void AddMechanism(string name)
    {
        Require.NotNullOrWhiteSpace(name);
        AddChild(new Element("mechanism", Namespace.Sasl));
    }

    public bool IsMechanismSupported(string name)
        => SupportedMechanisms?.Any(x => x.MechanismName == name) == true;

    public bool IsMechanismSupported(MechanismType type)
        => type != MechanismType.Unspecified && IsMechanismSupported(type.ToXmppName());
}