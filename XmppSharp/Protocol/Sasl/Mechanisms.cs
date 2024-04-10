using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanisms", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Mechanisms : XElement
{
    public Mechanisms() : base(Namespace.Sasl + "mechanisms")
    {

    }

    public IEnumerable<Mechanism> SupportedMechanisms
    {
        get => this.Elements<Mechanism>();
        set
        {
            Elements().ForEach(n => n.Remove());

            foreach (var item in value)
            {
                if (item != null)
                    Add(item);
            }
        }
    }
    public void AddMechanism(MechanismType type)
        => AddMechanism(type.ToXmppName());

    public void AddMechanism(string name)
    {
        Require.NotNullOrWhiteSpace(name);
        Add(new XElement(Namespace.Sasl + "mechanism", name));
    }

    public bool IsMechanismSupported(string name)
        => SupportedMechanisms?.Any(x => x.MechanismName == name) == true;

    public bool IsMechanismSupported(MechanismType type)
        => type != MechanismType.Unspecified && IsMechanismSupported(type.ToXmppName());
}