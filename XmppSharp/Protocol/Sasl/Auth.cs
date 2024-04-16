using XmppSharp.Attributes;

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
        get => GetAttribute("mechanism");
        set => SetAttribute("mechanism", value);
    }
}
