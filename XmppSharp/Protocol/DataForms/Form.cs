using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

/// <summary>
/// Represents a form element used within Data Forms in XMPP.
/// This class encapsulates the core structure and properties of a data form, defining its type, instructions, fields, and reported data.
/// </summary>
[XmppTag("x", Namespaces.DataForms)]
public class Form : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Form"/> class with default values.
    /// </summary>
    public Form() : base("x", Namespaces.DataForms)
    {

    }

    /// <summary>
    /// Gets or sets the type of the form, indicating its purpose or interaction model.
    /// </summary>
    public FormType Type
    {
        get => XmppEnum.ParseOrDefault(GetAttribute("type"), FormType.Form);
        set => SetAttribute("type", XmppEnum.ToXmppName(value));
    }

    /// <summary>
    /// Gets or sets optional instructions for users to explain how to interact with the form.
    /// </summary>
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

    /// <summary>
    /// Gets or sets an optional title for the form, providing a clear and concise overview of its content.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the reported data which can be understood as a "table header" describing the data to follow.
    /// </summary>
    public Reported? Reported
    {
        get => Child<Reported>();
        set => ReplaceChild(value);
    }

    /// <summary>
    /// Gets or sets the collection of fields within the form, representing the data elements to be displayed or collected.
    /// </summary>
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
