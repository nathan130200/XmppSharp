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

            return XmppEnum.FromXmlOrDefault(GetAttribute("type"), FieldType.TextSingle);
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
            {
                SetTag(x =>
                {
                    x.TagName = "desc";
                    x.Value = value;
                });
            }
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
                SetTag(x => x.TagName = "required");
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
                {
                    SetTag(x =>
                    {
                        x.TagName = "value";
                        x.Namespace = Namespaces.DataForms;
                        x.Value = item;
                    });
                }
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

    public void AddValue(object value, IFormatProvider? ifp = default)
    {
        ThrowHelper.ThrowIfNull(value);

        SetTag(x =>
        {
            x.TagName = "value";
            x.Namespace = Namespaces.DataForms;
            x.SetValue(value, ifp);
        });
    }

    public void AddOption(Option option)
        => AddChild(option);
}
