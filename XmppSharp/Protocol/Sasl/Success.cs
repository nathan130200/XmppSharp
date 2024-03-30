using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Represents a "success" element used in the Simple Authentication and Security Layer (SASL) negotiation within XMPP. 
/// <para>Receipt of this element indicates successful authentication.</para>
/// </summary>
[XmppTag("success", Namespaces.Sasl)]
public sealed class Success : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Success"/> class.
    /// </summary>
    public Success() : base("success", Namespaces.Sasl)
    {

    }
}
