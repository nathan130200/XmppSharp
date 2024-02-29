using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanisms", Namespaces.Sasl)]
public sealed class Mechanisms : Element
{
    public Mechanisms() : base("mechanisms", Namespaces.Sasl)
    {

    }

    public IEnumerable<Mechanism> SupportedAuthMechanisms
    {
        get => Elements().OfType<Mechanism>();
        set
        {
            RemoveAllChildren();

            if (value?.Any() == false)
                return;

            foreach (var mechanism in value)
                AddChild(mechanism);
        }
    }
}
