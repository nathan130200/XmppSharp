using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents an error element in XMPP stanzas.
/// </summary>
[Tag("error", Namespaces.Client)]
[Tag("error", Namespaces.Component)]
[Tag("error", Namespaces.Server)]
public class Error : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with the default namespace.
    /// </summary>
    public Error() : base("error", Stanza.DefaultNamespace)
    {

    }

    /// <summary>
    /// Gets or sets the type of the error.
    /// </summary>
    /// <remarks>
    /// This attribute indicates the nature of the error and is typically used to categorize errors for handling purposes.
    /// </remarks>
    public ErrorType Type
    {
        get => XmppEnum.ParseOrDefault<ErrorType>(GetAttribute("type"));
        set => SetAttribute("type", value.ToXmlOrDefault());
    }

    /// <summary>
    /// Gets or sets the condition of the error.
    /// </summary>
    public ErrorCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetMembers<ErrorCondition>())
            {
                if (HasTag(name, Namespaces.Stanzas))
                    return value;
            }

            return ErrorCondition.UndefinedCondition;
        }
        set
        {
            foreach (var name in XmppEnum.GetNames<ErrorCondition>())
                RemoveTag(name, Namespaces.Stanzas);

            if (Enum.IsDefined(value))
                SetTag(XmppEnum.ToXml(value)!, Namespaces.Stanzas);
        }
    }

    /// <summary>
    /// Gets or sets the error code, which is an optional attribute that provides additional information about the error.
    /// </summary>
    public int? Code
    {
        get => GetAttribute<int>("code");
        set => SetAttribute("code", value);
    }

    /// <summary>
    /// Gets or sets the custom error code, which is an optional attribute that can be used to provide application-specific error information.
    /// </summary>
    public int? CustomCode
    {
        get => GetAttribute<int>("custom_code");
        set => SetAttribute("custom_code", value);
    }

    /// <summary>
    /// Gets or sets the human-readable text description of the error, which is an optional element that provides a more detailed explanation of the error condition.
    /// </summary>
    public string? Text
    {
        get => GetTag("text", Namespaces.Stanzas);
        set
        {
            if (value is null)
                RemoveTag("text", Namespaces.Stanzas);
            else
                SetTag("text", Namespaces.Stanzas, value);
        }
    }
}