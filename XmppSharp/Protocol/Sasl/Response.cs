using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("response", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Response : XElement
{
    public Response() : base(Namespace.Sasl + "response")
    {

    }
}