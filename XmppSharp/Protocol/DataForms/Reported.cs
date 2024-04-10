using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("reported", "jabber:x:data")]
public class Reported : XElement
{
    public Reported() : base(Namespace.DataForms + "reported")
    {

    }

    public IEnumerable<Field> Fields
    {
        get => this.Elements<Field>();
        set
        {
            Fields.Remove();

            foreach (var item in value)
                Add(item);
        }
    }
}