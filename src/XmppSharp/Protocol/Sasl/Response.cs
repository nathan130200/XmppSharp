using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("response", Namespaces.Sasl)]
public class Response : Element
{
    public Response() : base("response", Namespaces.Sasl)
    {

    }
}
