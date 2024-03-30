using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("x", Namespaces.DataForms)]
public class Form : Element
{
    public Form() : base("x", Namespaces.DataForms)
    {

    }

    public FormType Type
    {
        get => XmppEnum.ParseOrDefault(GetAttribute("type"), FormType.Form);
        set => SetAttribute("type", XmppEnum.ToXmppName(value));
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
        set => ReplaceChild(value);
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
