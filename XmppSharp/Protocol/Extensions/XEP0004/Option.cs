using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0004;

[XmppTag("option", Namespaces.DataForms)]
public class Option : Element
{
    public Option() : base("option", Namespaces.DataForms)
    {

    }

    public Option(string? label, string? value) : this()
    {
        Label = label;
        Value = value;
    }

    public string? Label
    {
        get => GetAttribute("label");
        set => SetAttribute("label", value);
    }

    public new string? Value
    {
        get => GetTag("value");
        set
        {
            RemoveTag("value");

            if (value != null)
                SetTag("value", value);
        }
    }
}