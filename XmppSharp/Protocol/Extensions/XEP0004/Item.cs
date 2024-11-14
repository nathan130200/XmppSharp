using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0004;

[XmppTag("item", Namespaces.DataForms)]
public class Item : Element
{
    public Item() : base("item", Namespaces.DataForms)
    {

    }

    public IEnumerable<Field> Fields
    {
        get => Children<Field>();
        set
        {
            Children<Field>().Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }

    public void AddField(Field field)
        => AddChild(field);
}
