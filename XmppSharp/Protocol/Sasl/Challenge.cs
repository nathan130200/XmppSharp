using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("challenge", "urn:ietf:params:xml:ns:xmpp-sasl")]
public sealed class Challenge : XElement
{
    public Challenge() : base(Namespace.Sasl + "challenge")
    {

    }
}
