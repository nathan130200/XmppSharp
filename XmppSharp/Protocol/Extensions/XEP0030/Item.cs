using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0030;

[XmppTag("item", Namespaces.DiscoItems)]
public class Item : XmppElement
{
    public Item() : base("item", Namespaces.DiscoItems)
    {

    }

    public Jid? Jid
    {
        get => GetAttribute("jid");
        set => SetAttribute("jid", value);
    }

    public string? ItemName
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
