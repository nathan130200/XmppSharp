using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("iq", Namespaces.Client)]
[XmppTag("iq", Namespaces.Server)]
[XmppTag("iq", Namespaces.Accept)]
[XmppTag("iq", Namespaces.Connect)]
public class Iq : Stanza
{
    public Iq() : base("iq", Namespaces.Client)
    {

    }

    public Iq(IqType type) : this()
        => Type = type;

    public new IqType Type
    {
        get => XmppEnum.ParseOrThrow<IqType>(base.Type);
        set => base.Type = value.ToXmppName();
    }

    public Element Query
    {
        get
        {
            Element result;

            _ = TryGetChild("query", Namespaces.CryOnline, out result)
                || TryGetChild("bind", Namespaces.Bind, out result)
                || TryGetChild("session", Namespaces.Session, out result)
                || TryGetChild("query", Namespaces.DiscoInfo, out result)
                || TryGetChild("query", Namespaces.DiscoItems, out result)
                || TryGetChild("ping", Namespaces.Ping, out result);

            return result;
        }
        set
        {
            Query?.Remove();

            if (value != null)
                AddChild(value);
        }
    }
}