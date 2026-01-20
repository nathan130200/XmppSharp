using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("mechanisms", Namespaces.Sasl)]
public class Mechanisms : XmppElement
{
	public Mechanisms() : base("mechanisms", Namespaces.Sasl)
	{

	}

	public IEnumerable<Mechanism> SupportedMechanisms
	{
		get => Elements<Mechanism>();
		set
		{
			ArgumentNullException.ThrowIfNull(value);

			Elements<Mechanism>()?.Remove();

			if (value?.Any() == true)
			{
				foreach (var item in value)
					AddChild(item);
			}
		}
	}

	public bool SupportsMechanism(string name)
		=> SupportedMechanisms.Any(x => x.InnerText == name);

	public void AddMechanism(string name)
		=> AddChild(new Mechanism(name));
}
