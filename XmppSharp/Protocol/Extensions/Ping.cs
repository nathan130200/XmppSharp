using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions;

[XmppTag("ping", "urn:xmpp:ping")]
public class Ping : Element
{
    public Ping() : base("ping", Namespaces.Ping)
    {

    }
}