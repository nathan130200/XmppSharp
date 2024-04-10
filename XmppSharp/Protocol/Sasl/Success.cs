using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("success", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Success : XElement
{
    public Success() : base(Namespace.Sasl + "success")
    {

    }
}
