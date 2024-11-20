using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Sasl;

[XmppTag("mechanisms", Namespaces.Sasl)]
public class Mechanisms : Element
{
    public Mechanisms(Mechanisms other) : base(other)
    {

    }

    public Mechanisms() : base("mechanisms", Namespaces.Sasl)
    {

    }

    public IEnumerable<Mechanism> SupportedMechanisms
    {
        get => Children<Mechanism>();
        set
        {
            ThrowHelper.ThrowIfNull(value);

            Children<Mechanism>()?.Remove();

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
