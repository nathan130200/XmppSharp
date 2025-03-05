using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("response", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Response : XmppElement
{
    public Response() : base("response", Namespaces.Sasl)
    {

    }
}