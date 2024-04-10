using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Client;

[XmppTag("session", "urn:ietf:params:xml:ns:xmpp-session")]
public class Session : XElement
{
    public Session() : base(Namespace.Session + "session")
    {

    }
}