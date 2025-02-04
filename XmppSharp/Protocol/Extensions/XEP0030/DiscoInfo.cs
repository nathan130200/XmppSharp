using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0030;

[XmppTag("query", Namespaces.DiscoInfo)]
public class DiscoInfo : XmppElement
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
        get => Elements<Identity>();
        set
        {
            Elements<Identity>().Remove();

            if (value.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }

    public DiscoInfo AddIdentity(Identity identity)
    {
        AddChild(identity);
        return this;
    }

    public DiscoInfo AddFeature(Feature feature)
    {
        AddChild(feature);
        return this;
    }

    public IEnumerable<Feature> Features
    {
        get => Elements<Feature>();
        set
        {
            Elements<Feature>().Remove();

            if (value.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }
}
