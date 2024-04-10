using System.Xml.Linq;
using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("iq", "jabber:client")]
[XmppTag("iq", "jabber:server")]
[XmppTag("iq", "jabber:component:accept")]
[XmppTag("iq", "jabber:component:connect")]
public class Iq : Stanza
{
    public Iq() : base(Namespace.Client + "iq")
    {

    }

    public Iq(IqType type) : this()
        => Type = type;

    public new IqType Type
    {
        get => XmppEnum.ParseOrThrow<IqType>(base.Type);
        set => base.Type = value.ToXmppName();
    }

    public XElement Query
    {
        get
        {
            _ = this.TryGetChild(Namespace.CryOnline + "query", out XElement result)
                || this.TryGetChild(Namespace.Bind + "bind", out result)
                || this.TryGetChild(Namespace.Session + "session", out result)
                || this.TryGetChild(Namespace.DiscoInfo + "query", out result)
                || this.TryGetChild(Namespace.DiscoItems + "query", out result)
                || this.TryGetChild(Namespace.Ping + "ping", out result);

            return result;
        }
        set
        {
            Query?.Remove();

            if (value != null)
                Add(value);
        }
    }
}