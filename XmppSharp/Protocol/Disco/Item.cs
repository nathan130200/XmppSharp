using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Disco;

[XmppTag("item", Namespaces.DiscoItems)]
public class Item : Element
{
    public Item() : base("item", Namespaces.DiscoItems)
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
}