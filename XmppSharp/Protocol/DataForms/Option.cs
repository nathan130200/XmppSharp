using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("option", Namespace.DataForms)]
public class Option : Element
{
    public Option() : base("option", Namespace.DataForms)
    {

    }

    public string? Label
    {
        get => GetAttribute("label");
        set => SetAttribute("label", value);
    }

    public new string Value
    {
        get => GetTag("value");
        set => SetTag("value", value);
    }
}