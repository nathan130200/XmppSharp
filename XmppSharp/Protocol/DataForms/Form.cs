using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("x", "jabber:x:data")]
public class Form : XElement
{
    public Form() : base(Namespace.DataForms + "x")
    {

    }

    public FormType Type
    {
        get => XmppEnum.ParseOrDefault(this.GetAttribute("type"), FormType.Form);
        set => this.SetAttribute("type", value.ToXmppName());
    }

    public string? Instructions
    {
        get => this.GetTag("instructions");
        set
        {
            if (value == null)
                this.RemoveTag("instructions");
            else
                this.SetTag("instructions", value);
        }
    }

    public string? Title
    {
        get => this.GetTag("title");
        set
        {
            if (value == null)
                this.RemoveTag("title");
            else
                this.SetTag("title", value);
        }
    }

    public Reported? Reported
    {
        get => this.Element<Reported>();
        set
        {
            Reported?.Remove();
            Add(value);
        }
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
