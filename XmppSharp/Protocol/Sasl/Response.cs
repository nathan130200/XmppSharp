using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("response", Namespaces.Sasl)]
public sealed class Response : Element
{
    public Response() : base("response", Namespaces.Sasl)
    {

    }
}