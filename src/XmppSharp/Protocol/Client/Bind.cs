using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

[Tag("bind", Namespaces.Bind)]
public sealed class Bind() : Element("bind", Namespaces.Client)
{
    public Bind(Jid jid) : this()
    {
        Jid = jid;
    }

    public Bind(string? resource) : this()
    {
        Resource = resource;
    }

    public Jid? Jid
    {
        get => GetTag("jid", Namespaces.Bind);

        set
        {
            if (value is null)
                RemoveTag("jid", Namespaces.Bind);
            else
                SetTag("jid", Namespaces.Bind, value);
        }
    }

    public string? Resource
    {
        get => GetTag("resource", Namespaces.Bind);

        set
        {
            if (value is null)
                RemoveTag("resource", Namespaces.Bind);
            else
                SetTag("resource", Namespaces.Bind, value);
        }
    }
}