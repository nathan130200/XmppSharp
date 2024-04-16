using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("x", Namespace.DataForms)]
public class Form : Element
{
    public Form() : base("x", Namespace.DataForms)
    {

    }

    public FormType Type
    {
        get => XmppEnum.ParseOrDefault(GetAttribute("type"), FormType.Form);
        set => SetAttribute("type", value.ToXmppName());
    }

    public string? Instructions
    {
        get => GetTag("instructions");
        set
        {
            if (value == null)
                RemoveTag("instructions");
            else
                SetTag("instructions", value);
        }
    }

    public string? Title
    {
        get => GetTag("title");
        set
        {
            if (value == null)
                RemoveTag("title");
            else
                SetTag("title", value);
        }
    }

    public Reported? Reported
    {
        get => Child<Reported>();
        set
        {
            Reported?.Remove();
            AddChild(value);
        }
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
