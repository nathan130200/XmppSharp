using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

/// <summary>
/// Represents a field within a data form in XMPP.
/// This class encapsulates the properties and values associated with a single field, defining its unique identifier, type, label, description, options, and value.
/// </summary>
[XmppTag("field", Namespaces.DataForms)]
public class Field : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Field"/> class with default values.
    /// </summary>
    public Field() : base("field", Namespaces.DataForms)
    {

    }

    /// <summary>
    /// Gets or sets the variable name that uniquely identifies this field within the form.
    /// </summary>
    public string? Name
    {
        get => GetAttribute("var");
        set => SetAttribute("var", value);
    }

    /// <summary>
    /// Gets or sets the type of the field, determining the expected format and behavior of user input.
    /// </summary>
    public FieldType Type
    {
        get => XmppEnum.ParseOrDefault(GetAttribute("type"), FieldType.TextSingle);
        set => SetAttribute("type", value.ToXmppName());
    }

    /// <summary>
    /// Gets or sets a human-readable label for the field, displayed to the user to provide context.
    /// </summary>
    public string? Label
    {
        get => GetAttribute("label");
        set => SetAttribute("label", value);
    }

    /// <summary>
    /// Gets or sets an optional description of the field, providing additional information or instructions.
    /// </summary>
    public string? Description
    {
        get => GetTag("desc");
        set => SetTag("desc", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field is required to be filled in by the user.
    /// </summary>
    public bool IsRequired
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

    /// <summary>
    /// Gets or sets the current value of the field, representing the data entered or selected by the user.
    /// </summary>
    public string Value
    {
        get => GetTag("value");
        set => SetTag("value", value);
    }

    /// <summary>
    /// Gets or sets a collection of options associated with the field, typically used for list-based fields.
    /// </summary>
    public IEnumerable<Option> Option
    {
        get => Children<Option>();
        set
        {
            Children<Option>().Remove();

            foreach (var item in value)
                AddChild(item);
        }
    }
}
