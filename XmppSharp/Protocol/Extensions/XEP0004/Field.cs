using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0004;

[XmppTag("field", Namespaces.DataForms)]
public class Field : Element
{
    public Field() : base("field", Namespaces.DataForms)
    {

    }

    public Field(FieldType type, string? label = default, string? @var = default, string? desc = default) : this()
    {
        Type = type;
        Label = label;
        Var = @var;
        Description = desc;
    }

    public FieldType? Type
    {
        get
        {
            if (!HasAttribute("type"))
                return null;

            return XmppEnum.FromXml(GetAttribute("type"), FieldType.TextSingle);
        }
        set
        {
            var type = value.GetValueOrDefault(FieldType.TextSingle);

            if (!Enum.IsDefined(type))
                type = FieldType.TextSingle;

            SetAttribute("type", XmppEnum.ToXml(type));
        }
    }

    public string? Label
    {
        get => GetAttribute("label");
        set => SetAttribute("label", value);
    }

    public string? Var
    {
        get => GetAttribute("var");
        set => SetAttribute("var", value);
    }

    public string? Description
    {
        get => GetTag("desc");
        set
        {
            RemoveTag("desc");

            if (value != null)
                SetTag("desc", value: value);
        }
    }

    public bool Required
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

    public IEnumerable<string?> Values
    {
        get => Children("value").Select(x => x.Value);
        set
        {
            Children("value")?.Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    SetTag("value", Namespaces.DataForms, item);
            }
        }
    }

    public IEnumerable<Option> Options
    {
        get => Children<Option>();
        set
        {
            Children<Option>().Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }

    public void AddValue(object value)
    {
        ThrowHelper.ThrowIfNull(value);
        SetTag("value", Namespaces.DataForms, value);
    }

    public void AddOption(Option option)
        => AddChild(option);
}
