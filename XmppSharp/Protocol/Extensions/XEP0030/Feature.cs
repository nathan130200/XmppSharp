using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0030;

[XmppTag("feature", Namespaces.DiscoInfo)]
public class Feature : XmppElement
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
