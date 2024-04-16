using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("ping", "urn:xmpp:ping")]
public class Ping : Element
{
    public Ping() : base("ping", Namespace.Ping)
    {

    }
}
