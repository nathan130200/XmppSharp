using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("query", Namespace.DiscoItems)]
public class DiscoItems : Element
{
    public DiscoItems() : base("query", Namespace.DiscoItems)
    {

    }

    public DiscoItems(string? node) : this()
    {
        Node = node;
    }

    public string? Node
    {
        get => GetAttribute("node");
        set => SetAttribute("node", value);
    }

    public IEnumerable<Item> Items
    {
        get => Children<Item>();
        set
        {
            Children<Item>().Remove();

            if (value != null)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }
}
