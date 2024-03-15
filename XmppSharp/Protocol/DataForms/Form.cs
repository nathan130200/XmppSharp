using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("x", Namespace.DataForms)]
public class Form : Element
{
    public Form() : base("x", Namespace.DataForms)
    {

    }

    public FormType Type
    {
        get => XmppEnum.FromXml(GetAttribute("type"), FormType.Form);
        set => SetAttribute("type", XmppEnum.ToXml(value));
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
