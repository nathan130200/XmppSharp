using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("response", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Response : Element
{
	public Response() : base("response", Namespace.Sasl)
	{

	}
}