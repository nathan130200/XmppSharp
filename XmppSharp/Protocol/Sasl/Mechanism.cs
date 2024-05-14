using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanism", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Mechanism : Element
{
	public Mechanism() : base("mechanism", Namespaces.Sasl)
	{
		this.MechanismType = MechanismType.Plain;
	}

	public Mechanism(MechanismType type) : this()
		=> this.MechanismType = type;

	public Mechanism(string mechanismName) : this()
	{
		Require.NotNullOrWhiteSpace(mechanismName);
		this.MechanismName = mechanismName;
	}

	public MechanismType MechanismType
	{
		get => XmppEnum.ParseOrDefault(this.Value, MechanismType.Unspecified);
		set
		{
			if (!Enum.IsDefined(value) || value == MechanismType.Unspecified)
				this.MechanismName = default;
			else
				this.MechanismName = value.ToXmppName();
		}
	}

	public string? MechanismName
	{
		get => this.Value;
		set => this.Value = value ?? string.Empty;
	}
}
