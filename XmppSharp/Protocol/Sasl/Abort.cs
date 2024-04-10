using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("abort", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Abort : XElement
{
    public Abort() : base(Namespace.Sasl + "abort")
    {

    }
}