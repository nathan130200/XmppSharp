using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("success", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Success : Element
{
	public Success() : base("success", Namespaces.Sasl)
	{

	}
}
