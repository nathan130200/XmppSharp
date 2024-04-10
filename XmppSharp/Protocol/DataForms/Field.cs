using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("field", "jabber:x:data")]
public class Field : XElement
{
    public Field() : base(Namespace.DataForms + "field")
    {

    }

    public new string? Name
    {
        get => this.GetAttribute("var");
        set => this.SetAttribute("var", value);
    }

    public FieldType Type
    {
        get => XmppEnum.ParseOrDefault(this.GetAttribute("type"), FieldType.TextSingle);
        set => this.SetAttribute("type", value.ToXmppName());
    }

    public string? Label
    {
        get => this.GetAttribute("label");
        set => this.SetAttribute("label", value);
    }

    public string? Description
    {
        get => this.GetTag("desc");
        set => this.SetTag("desc", value);
    }

    public bool IsRequired
    {
        get => this.HasTag("required");
        set
        {
            if (!value)
                this.RemoveTag("required");
            else
                this.SetTag("required");
        }
    }

    public new string Value
    {
        get => this.GetTag("value");
        set => this.SetTag("value", value);
    }

    public IEnumerable<Option> Option
    {
        get => this.Elements<Option>();
        set
        {
            this.Elements<Option>().Remove();

            foreach (var item in value)
            {
                if (item != null)
                    Add(item);
            }
        }
    }
}
