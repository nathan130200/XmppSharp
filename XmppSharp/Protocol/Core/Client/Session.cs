using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Client;

[XmppTag("session", Namespaces.Session)]
public class Session : Element
{
    public Session() : base("session", Namespaces.Session)
    {

    }
}