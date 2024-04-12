using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("query", "http://jabber.org/protocol/disco#items")]
public class DiscoItems : XElement
{
    public DiscoItems() : base(Namespace.DiscoItems + "query")
    {

    }

    public DiscoItems(string? node) : this()
    {
        Node = node;
    }

    public string? Node
    {
        get => this.GetAttribute("node");
        set => this.SetAttribute("node", value);
    }

    public IEnumerable<Item> Items
    {
        get => this.Elements<Item>();
        set
        {
            this.Elements<Item>().Remove();

            if (value != null)
            {
                foreach (var item in value)
                    Add(item);
            }
        }
    }
}
