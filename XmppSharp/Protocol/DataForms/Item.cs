using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("item", "jabber:x:data")]
public class Item : XElement
{
    public Item() : base("item", Namespace.DataForms)
    {

    }

    public Field? Field
    {
        get => this.Element<Field>();
        set
        {
            Field?.Remove();
            Add(value);
        }
    }
}
