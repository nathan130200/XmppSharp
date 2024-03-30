using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Represents a "response" element used in the Simple Authentication and Security Layer (SASL) negotiation within XMPP.
/// <para>This element typically carries the client's response data during the authentication process.</para>
/// </summary>
[XmppTag("response", Namespaces.Sasl)]
public sealed class Response : Element
{
    public Response() : base("response", Namespaces.Sasl)
    {

    }
}