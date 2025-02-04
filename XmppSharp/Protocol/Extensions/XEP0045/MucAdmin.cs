using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[XmppTag("query", Namespaces.MucAdmin)]
public class MucAdmin : XmppElement
{
    public MucAdmin() : base("query", Namespaces.MucAdmin)
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
}
