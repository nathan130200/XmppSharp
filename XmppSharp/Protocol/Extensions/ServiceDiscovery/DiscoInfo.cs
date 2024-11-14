using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.ServiceDiscovery;

[XmppTag("query", Namespaces.DiscoInfo)]
public class DiscoInfo : Element
{
    public DiscoInfo() : base("query", Namespaces.DiscoInfo)
    {

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

            if (value.Any() == true)
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

            if (value.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }
}
