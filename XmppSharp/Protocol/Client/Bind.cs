using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

[XmppTag("bind", Namespaces.Bind)]
public class Bind : XmppElement
{
    public Bind() : base("bind", Namespaces.Bind)
    {

    }

    public Bind(string? resource) : this()
        => Resource = resource;

    public Bind(Jid jid) : this()
        => Jid = jid;

    public string? Resource
    {
        get => GetTag("resource", Namespaces.Bind);
        set
        {
            RemoveTag("resource", Namespaces.Bind);

            if (value != null)
                SetTag("resource", Namespaces.Bind, value);
        }
    }

    public Jid? Jid
    {
        get => GetTag("jid", Namespaces.Bind);
        set
        {
            RemoveTag("jid", Namespaces.Bind);

            if (value != null)
                SetTag("jid", Namespaces.Bind, value);
        }
    }
}
