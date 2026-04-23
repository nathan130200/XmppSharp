using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

/// <summary>
/// The <![CDATA[<proceed>]]> element is sent by the server to the client to indicate that the TLS negotiation can proceed.
/// </summary>
[Tag("proceed", Namespaces.Tls)]
public class Proceed : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Proceed"/> class.
    /// </summary>
    public Proceed() : base("proceed", Namespaces.Tls)
    {

    }
}
