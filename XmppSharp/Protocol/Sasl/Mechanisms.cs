using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanisms", Namespace.Sasl)]
public class Mechanisms : Element
{
	public Mechanisms() : base("mechanisms", Namespace.Sasl)
	{

	}

	public IEnumerable<Mechanism> SupportedMechanisms
	{
		get => this.Children<Mechanism>();
		set
		{
			this.Children().Remove();

			foreach (var item in value)
				this.AddChild(item);
		}
	}

	public void AddMechanism(MechanismType type)
	{
		if (type == MechanismType.Unspecified || !Enum.IsDefined(type))
			return;

		this.AddMechanism(type.ToXmppName());
	}

	public void AddMechanism(string name)
	{
		Require.NotNullOrWhiteSpace(name);
		this.AddChild(new Element("mechanism", Namespace.Sasl));
	}

	public bool IsMechanismSupported(string name)
		=> this.SupportedMechanisms?.Any(x => x.MechanismName == name) == true;

	public bool IsMechanismSupported(MechanismType type)
		=> type != MechanismType.Unspecified && this.IsMechanismSupported(type.ToXmppName());
}