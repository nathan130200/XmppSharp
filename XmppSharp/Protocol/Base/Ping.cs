using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

[XmppTag("ping", Namespaces.Ping)]
public class Ping : Element
{
    public Ping() : base("ping", Namespaces.Ping)
    {

    }
}
