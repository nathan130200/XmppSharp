using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("mechanisms", Namespaces.Sasl)]
public sealed class Mechanisms() : Element("mechanisms", Namespaces.Sasl)
{
	public bool SupportsMechanism(string name)
	{
		return Elements<Mechanisms>().Any(x => x.InnerText == name);
	}

	public void AddMechanism(string name)
	{
		if (SupportsMechanism(name)) return;

		AddChild(new Mechanism(name));
	}

	public void RemoveAllMechanisms()
	{
		RemoveTag("mechanism", Namespaces.Sasl);
	}
}