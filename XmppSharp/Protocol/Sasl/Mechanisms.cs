using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanisms", Namespaces.Sasl)]
public class Mechanisms : XmppElement
{
    public Mechanisms(Mechanisms other) : base(other)
    {

    }

    public Mechanisms() : base("mechanisms", Namespaces.Sasl)
    {

    }

    public IEnumerable<Mechanism> SupportedMechanisms
    {
        get => Elements<Mechanism>();
        set
        {
            Throw.IfNull(value);

            Elements<Mechanism>()?.Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }

    public bool HasMechanism(string name)
        => SupportedMechanisms.Any(x => x.Value == name);

    public void AddMechanism(string name)
        => AddChild(new Mechanism(name));
}
