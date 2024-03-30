using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("option", Namespaces.DataForms)]
public class Option : Element
{
    public Option() : base("option", Namespaces.DataForms)
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