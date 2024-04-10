using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("ping", "urn:xmpp:ping")]
public class Ping : XElement
{
    public Ping() : base(Namespace.Ping + "ping")
    {

    }
}
