using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("item", "http://jabber.org/protocol/disco#items")]
public class Item : XElement
{
    public Item() : base(Namespace.DiscoItems + "item")
    {

    }

    public Item(Jid? jid, string? name) : this()
    {
        Jid = jid;
        Name = name;
    }

    public Jid? Jid
    {
        get => this.GetAttribute("jid");
        set => this.SetAttribute("jid", value);
    }

    public new string? Name
    {
        get => this.GetAttribute("name");
        set => this.SetAttribute("name", value);
    }

    public string? Node
    {
        get => this.GetAttribute("node");
        set => this.SetAttribute("node", value);
    }
}