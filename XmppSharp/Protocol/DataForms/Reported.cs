using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

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
            Children<Field>().Remove();

            foreach (var element in value)
                AddChild(element);
        }
    }
}