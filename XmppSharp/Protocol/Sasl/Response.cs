using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("response", Namespace.Sasl)]
public sealed class Response : Element
{
    public Response() : base("response", Namespace.Sasl)
    {

    }
}