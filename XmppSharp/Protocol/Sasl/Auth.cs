using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("auth", Namespaces.Sasl)]
public sealed class Auth : Element
{
	public Auth() : base("auth", Namespaces.Sasl)
	{

	}

	public Auth(MechanismType type) : this()
		=> this.MechanismType = type;

	public Auth(string name) : this()
		=> this.MechanismName = name;

	public MechanismType? MechanismType
	{
		get => XmppEnum.Parse<MechanismType>(this.MechanismName);
		set
		{
			if (!value.TryGetValue(out var result))
				this.MechanismName = null;
			else
				this.MechanismName = result.ToXmppName();
		}
	}

	public string? MechanismName
	{
		get => this.GetAttribute("mechanism");
		set => this.SetAttribute("mechanism", value);
	}
}
