using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("query", Namespace.DiscoInfo)]
public class DiscoInfo : Element
{
    public DiscoInfo() : base("query", Namespace.DiscoInfo)
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
        get => GetAttribute("node");
        set => SetAttribute("node", value);
    }

    public IEnumerable<Identity> Identities
    {
        get => Children<Identity>();
        set
        {
            Children<Identity>().Remove();

            if (value != null)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }

    public IEnumerable<Feature> Features
    {
        get => Children<Feature>();
        set
        {
            Children<Feature>().Remove();

            if (value != null)
            {
                foreach (var feature in value)
                    AddChild(feature);
            }
        }
    }
}
