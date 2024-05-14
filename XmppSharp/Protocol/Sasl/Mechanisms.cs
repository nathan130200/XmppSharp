using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanisms", Namespaces.Sasl)]
public class Mechanisms : Element
{
	public Mechanisms() : base("mechanisms", Namespaces.Sasl)
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
		this.AddChild(new Element("mechanism", Namespaces.Sasl));
	}

	public bool IsMechanismSupported(string name)
		=> this.SupportedMechanisms?.Any(x => x.MechanismName == name) == true;

	public bool IsMechanismSupported(MechanismType type)
		=> type != MechanismType.Unspecified && this.IsMechanismSupported(type.ToXmppName());
}