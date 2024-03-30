using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Represents an "abort" element used in Simple Authentication and Security Layer (SASL) negotiation within XMPP.
/// <para>This element indicates that a SASL negotiation has been aborted or canceled by either the client or the server.</para>
/// </summary>
/// <remarks>
/// It does not typically contain any additional data, simply serving as a notification of termination.
/// </remarks>
[XmppTag("abort", Namespaces.Sasl)]
public class Abort : Element
{
    public Abort() : base("abort", Namespaces.Sasl)
    {

    }
}