using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Client;

[XmppTag("session", Namespace.Session)]
public class Session : Element
{
    public Session() : base("session", Namespace.Session)
    {

    }
}