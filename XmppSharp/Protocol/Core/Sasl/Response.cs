using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Sasl;

[XmppTag("response", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Response : Element
{
    public Response() : base("response", Namespaces.Sasl)
    {

    }
}