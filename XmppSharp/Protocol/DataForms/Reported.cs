using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("reported", Namespace.DataForms)]
public class Reported : Element
{
    public Reported() : base("reported", Namespace.DataForms)
    {

    }

    public IEnumerable<Field> Fields
    {
        get => Children<Field>();
        set
        {
            Fields.Remove();

            foreach (var item in value)
                AddChild(item);
        }
    }
}