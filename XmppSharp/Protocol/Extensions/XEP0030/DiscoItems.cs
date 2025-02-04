using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0030;

[XmppTag("query", Namespaces.DiscoItems)]
public class DiscoItems : XmppElement
{
    public DiscoItems() : base("query", Namespaces.DiscoItems)
    {

    }

    public IEnumerable<Item> Items
    {
        get => Elements<Item>();
        set
        {
            Elements<Item>()?.Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }

    public DiscoItems AddItem(Item item)
    {
        AddChild(item);
        return this;
    }
}
