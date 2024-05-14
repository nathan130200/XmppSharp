using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("abort", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Abort : Element
{
	public Abort() : base("abort", Namespaces.Sasl)
	{

	}
}