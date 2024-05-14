using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("challenge", "urn:ietf:params:xml:ns:xmpp-sasl")]
public sealed class Challenge : Element
{
	public Challenge() : base("challenge", Namespaces.Sasl)
	{

	}
}
