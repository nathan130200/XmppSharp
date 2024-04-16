using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("item", Namespace.DiscoItems)]
public class Item : Element
{
    public Item() : base("item", Namespace.DiscoItems)
    {

    }

    public Item(Jid? jid, string? name) : this()
    {
        Jid = jid;
        Name = name;
    }

    public Jid? Jid
    {
        get => GetAttribute("jid");
        set => SetAttribute("jid", value);
    }

    public string? Name
    {
        get => GetAttribute("name");
        set => SetAttribute("name", value);
    }

    public string? Node
    {
        get => GetAttribute("node");
        set => SetAttribute("node", value);
    }
}