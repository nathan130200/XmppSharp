using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Represents a "challenge" element used in Simple Authentication and Security Layer (SASL) negotiation within XMPP.
/// <para>This element typically contains a challenge from the server to the client as part of a challenge-response authentication mechanism.</para>
/// </summary>
[XmppTag("challenge", Namespaces.Sasl)]
public sealed class Challenge : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Challenge"/> class.
    /// </summary>
    public Challenge() : base("challenge", Namespaces.Sasl)
    {

    }
}
