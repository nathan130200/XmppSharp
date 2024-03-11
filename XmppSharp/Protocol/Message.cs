using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol;

[XmppTag("message", Namespace.Client)]
[XmppTag("message", Namespace.Accept)]
[XmppTag("message", Namespace.Server)]
public class Message : Element
{
    public Message() : base("message")
    {

    }
}

