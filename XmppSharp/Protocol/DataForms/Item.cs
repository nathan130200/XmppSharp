using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("item", Namespace.DataForms)]
public class Item : Element
{
    public Item() : base("item", Namespace.DataForms)
    {

    }

    public Field? Field
    {
        get => Child<Field>();
        set => ReplaceChild(value);
    }
}
