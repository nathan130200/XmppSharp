using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

[XmppTag("proceed", "urn:ietf:params:xml:ns:xmpp-tls")]
public sealed class Proceed : XElement
{
    public Proceed() : base(Namespace.Tls + "proceed")
    {

    }
}
