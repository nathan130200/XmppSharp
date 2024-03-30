using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("field", Namespaces.DataForms)]
public class Field : Element
{
    public Field() : base("field", Namespaces.DataForms)
    {

    }

    public string? Var
    {
        get => GetAttribute("var");
        set => SetAttribute("var", value);
    }

    public FieldType Type
    {
        get => XmppEnum.ParseOrDefault(GetAttribute("type"), FieldType.TextSingle);
        set => SetAttribute("type", XmppEnum.ToXmppName(value));
    }

    public string? Label
    {
        get => GetAttribute("label");
        set => SetAttribute("label", value);
    }

    public string Description
    {
        get => GetTag("desc");
        set => SetTag("desc", value);
    }

    public bool IsRequired
    {
        get => HasTag("required");
        set
        {
            if (!value)
                RemoveTag("required");
            else
                SetTag("required");
        }
    }

    public new string Value
    {
        get => GetTag("value");
        set => SetTag("value", value);
    }

    public IEnumerable<Option> Option
    {
        get => Children<Option>();
        set
        {
            Children<Option>().Remove();

            foreach (var item in value)
                AddChild(item);
        }
    }
}
