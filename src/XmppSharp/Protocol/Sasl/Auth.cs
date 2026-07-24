using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("auth", Namespaces.Sasl)]
public class Auth() : Element("auth", Namespaces.Sasl)
{
    public Auth(string mechanism) : this()
    {
        Mechanism = mechanism;
    }

    public string? Mechanism
    {
        get => GetAttribute("mechanism");
        set => SetAttribute("mechanism", value);
    }
}