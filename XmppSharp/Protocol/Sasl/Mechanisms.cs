using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanisms", Namespaces.Sasl)]
public class Mechanisms : Element
{
    public Mechanisms() : base("mechanisms", Namespaces.Sasl)
    {

    }

    public IEnumerable<Mechanism> SupportedMechanisms
    {
        get => Children().OfType<Mechanism>();
        set
        {
            Descendants().ForEach(x => x.Remove());

            foreach (var mechanism in value)
                AddChild(mechanism);
        }
    }

    public void AddMechanism(Mechanism mechanism)
        => AddChild(mechanism);

    public void AddMechanism(MechanismType type)
        => AddMechanism(mechanism: new(type));

    public void AddMechanism(string name)
        => AddMechanism(mechanism: new(name));

    public bool IsMechanismSupported(string name)
        => SupportedMechanisms?.Any(x => x.MechanismName == name) == true;

    public bool IsMechanismSupported(MechanismType type)
        => IsMechanismSupported(type.ToXmppName());
}