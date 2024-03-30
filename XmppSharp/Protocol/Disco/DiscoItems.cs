using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Disco;

[XmppTag("query", Namespaces.DiscoItems)]
public class DiscoItems : Element
{
    public DiscoItems() : base("query", Namespaces.DiscoItems)
    {

    }

    public DiscoItems(params Item[] items) : this()
    {
        Items = items;
    }

    public DiscoItems(string node, params Item[] items) : this(items)
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
            Children().Remove();

            if (value != null)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }
}
