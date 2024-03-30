using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("reported", Namespaces.DataForms)]
public class Reported : Element
{
    public Reported() : base("reported", Namespaces.DataForms)
    {

    }

    public IEnumerable<Field> Fields
    {
        get => Children<Field>();
        set
        {
            Children<Field>().Remove();

            foreach (var element in value)
                AddChild(element);
        }
    }
}