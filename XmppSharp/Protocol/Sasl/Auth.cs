using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("auth", Namespaces.Sasl)]
public class Auth : XmppElement
{
    public Auth(Auth other) : base(other)
    {
    }

    public Auth() : base("auth", Namespaces.Sasl)
    {

    }

    public Auth(string mechanismName) : this()
    {
        Mechanism = mechanismName;
    }

    public string Mechanism
    {
        get => GetAttribute("mechanism")!;
        set
        {
            Throw.IfStringNullOrWhiteSpace(value, nameof(Mechanism));
            SetAttribute("mechanism", value);
        }
    }
}
