using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("mechanism", Namespaces.Sasl)]
public class Mechanism : XmppElement, ISaslMechanismInfo
{
	public Mechanism() : base("mechanism", Namespaces.Sasl)
	{

	}

	public Mechanism(string mechanismName) : this()
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(mechanismName);
		InnerText = mechanismName;
	}

	string? ISaslMechanismInfo.MechanismName => InnerText;
}
