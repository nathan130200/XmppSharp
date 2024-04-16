using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("feature", Namespace.DiscoInfo)]
public class Feature : Element
{
    public Feature() : base("feature", Namespace.DiscoInfo)
    {

    }

    public Feature(string featureName) : this()
    {
        Name = featureName;
    }

    public string Name
    {
        get => GetAttribute("var");
        set => SetAttribute("var", value);
    }
}