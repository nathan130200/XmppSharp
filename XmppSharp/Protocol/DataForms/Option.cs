using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

/// <summary>
/// Represents an "option" element used within Data Forms in XMPP.
/// <para>This element defines an option within a selection field, providing a human-readable label and associated value.</para>
/// </summary>
[XmppTag("option", Namespaces.DataForms)]
public class Option : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Option"/> class.
    /// </summary>
    public Option() : base("option", Namespaces.DataForms)
    {

    }

    /// <summary>
    /// Gets or sets the human-readable label for the option.
    /// This label is typically displayed to the user to present a clear choice.
    /// </summary>
    public string? Label
    {
        get => GetAttribute("label");
        set => SetAttribute("label", value);
    }

    /// <summary>
    /// Gets or sets the value associated with the option.
    /// This value is typically used for data submission or processing when the option is selected.
    /// </summary>
    public new string Value
    {
        get => GetTag("value");
        set => SetTag("value", value);
    }
}