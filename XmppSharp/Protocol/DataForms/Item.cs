using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("item", Namespaces.DataForms)]
public class Item : Element
{
    public Item() : base("item", Namespaces.DataForms)
    {

    }

    public Field? Field
    {
        get => Child<Field>();
        set => ReplaceChild(value);
    }
}
