using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("auth", Namespace.Sasl)]
public sealed class Auth : Element
{
    public Auth() : base("auth", Namespace.Sasl)
    {

    }

    public Auth(MechanismType type) : this()
        => MechanismType = type;

    public Auth(string name) : this()
        => MechanismName = name;

    public MechanismType MechanismType
    {
        get => XmppEnum.FromXml(MechanismName, MechanismType.Unspecified);
        set => MechanismName = value != MechanismType.Unspecified
            ? XmppEnum.ToXml(value) : null;
    }

    public string MechanismName
    {
        get => GetAttribute("mechanism");
        set => SetAttribute("mechanism", value);
    }
}
