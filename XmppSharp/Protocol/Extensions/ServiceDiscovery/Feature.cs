using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.ServiceDiscovery;

[XmppTag("feature", Namespaces.DiscoInfo)]
public class Feature : Element
{
    public Feature() : base("feature", Namespaces.DiscoInfo)
    {

    }

    public Feature(string @var) : this()
    {
        Var = @var;
    }

    public string? Var
    {
        get => GetAttribute("var");
        set => SetAttribute("var", value);
    }
}
