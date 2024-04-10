using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("query", "http://jabber.org/protocol/disco#info")]
public class DiscoInfo : XElement
{
    public DiscoInfo() : base(Namespace.DiscoInfo + "query")
    {
    }

    public DiscoInfo(string? node) : this()
    {
        Node = node;
    }

    public DiscoInfo(string? node, IEnumerable<Identity>? identities, IEnumerable<Feature>? features) : this(node)
    {
        Identities = identities;
        Features = features;
    }

    public string? Node
    {
        get => this.GetAttribute("node");
        set => this.SetAttribute("node", value);
    }

    public IEnumerable<Identity> Identities
    {
        get => this.Elements<Identity>();
        set
        {
            this.Elements<Identity>().Remove();

            if (value != null)
            {
                foreach (var item in value)
                {
                    if (item != null)
                        Add(item);
                }
            }
        }
    }

    public IEnumerable<Feature> Features
    {
        get => this.Elements<Feature>();
        set
        {
            this.Elements<Feature>().Remove();

            if (value != null)
            {
                foreach (var feature in value)
                {
                    if (feature != null)
                        Add(feature);
                }
            }
        }
    }
}
