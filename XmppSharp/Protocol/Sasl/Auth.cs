using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("auth", "urn:ietf:params:xml:ns:xmpp-sasl")]
public sealed class Auth : XElement
{
    public Auth() : base(Namespace.Sasl + "auth")
    {

    }

    public Auth(MechanismType type) : this()
        => MechanismType = type;

    public Auth(string name) : this()
        => MechanismName = name;

    public MechanismType? MechanismType
    {
        get => XmppEnum.Parse<MechanismType>(MechanismName);
        set
        {
            if (!value.TryGetValue(out var result))
                MechanismName = null;
            else
                MechanismName = result.ToXmppName();
        }
    }

    public string MechanismName
    {
        get => this.GetAttribute("mechanism");
        set => this.SetAttribute("mechanism", value);
    }
}
