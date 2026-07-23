using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("mechanism", Namespaces.Sasl)]
public class Mechanism() : Element("mechanism", Namespaces.Sasl)
{
	public Mechanism(string mechanismName) : this()
	{
		InnerText = mechanismName;
	}
}