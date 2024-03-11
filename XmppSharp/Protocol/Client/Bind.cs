using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Client;

[XmppTag("bind", Namespace.Bind)]
public class Bind : Element
{
    public Bind() : base("bind", Namespace.Bind)
    {

    }

    public string Resource
    {
        get => GetTag("resource");
        set => SetTag("resource", value);
    }

    public Jid Jid
    {
        get => Jid.Parse(GetTag("jid"), false);
        set => GetTag("jid", value?.ToString());
    }
}
