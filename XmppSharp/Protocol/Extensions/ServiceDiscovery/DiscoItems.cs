using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.ServiceDiscovery;

[XmppTag("query", Namespaces.DiscoItems)]
public class DiscoItems : Element
{
    public DiscoItems() : base("query", Namespaces.DiscoItems)
    {

    }

    public IEnumerable<Item> Items
    {
        get => Children<Item>();
        set
        {
            Children<Item>()?.Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }
}
