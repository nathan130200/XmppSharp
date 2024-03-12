using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Client;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol;

[XmppTag("iq", Namespace.Client)]
[XmppTag("iq", Namespace.Server)]
[XmppTag("iq", Namespace.Accept)]
[XmppTag("iq", Namespace.Connect)]
public class Iq : Stanza
{
    public Iq() : base("iq")
    {

    }

    public Iq(IqType type)
        => Type = type;

    public new IqType? Type
    {
        get => XmppEnum.FromXml<IqType>(base.Type);
        set
        {
            if (!value.TryGetValue(out var result))
                base.Type = null;
            else
                base.Type = XmppEnum.ToXml(result);
        }
    }

    public Bind Bind
    {
        get => Child<Bind>();
        set => ReplaceChild(value);
    }

    public Session Session
    {
        get => Child<Session>();
        set => ReplaceChild(value);
    }

    public Element GetQuery()
        => GetChild("query");

    public void SetQuery(Element e)
        => ReplaceChild(e);

    public Q GetQuery<Q>() where Q : Element
        => Child<Q>();

    public void SetQuery<Q>(Q value) where Q : Element
        => ReplaceChild(value);
}