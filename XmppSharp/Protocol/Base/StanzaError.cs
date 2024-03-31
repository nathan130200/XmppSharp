using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents an error in an XMPP stanza, indicating the type of error, condition, and optional text explanation.
/// </summary>
[XmppTag("error", Namespaces.Client)]
[XmppTag("error", Namespaces.Accept)]
[XmppTag("error", Namespaces.Server)]
public class StanzaError : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StanzaError"/> class with the default "error" element name.
    /// </summary>
    public StanzaError() : base("error", Namespaces.Client)
    {

    }

    /// <summary>
    /// Gets or sets the type of error.
    /// </summary>
    public StanzaErrorType? Type
    {
        get => XmppEnum.Parse<StanzaErrorType>(GetAttribute("type"));
        set
        {
            if (!value.TryUnwrap(out var result))
                RemoveAttribute("type");
            else
                SetAttribute("type", result.ToXmppName());
        }
    }

    /// <summary>
    /// Gets or sets the specific condition that caused the error.
    /// </summary>
    public StanzaErrorCondition? Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetValues<StanzaErrorCondition>())
            {
                if (HasTag(name))
                    return value;
            }

            return default;
        }
        set
        {
            foreach (var tag in XmppEnum.GetNames<StanzaErrorCondition>())
                RemoveTag(tag, Namespaces.Stanzas);

            if (value.TryUnwrap(out var result))
                SetTag(result.ToXmppName(), xmlns: Namespaces.Stanzas);
        }
    }

    /// <summary>
    /// Gets or sets the optional human-readable text providing further explanation of the error.
    /// </summary>
    public string Text
    {
        get => GetTag("text", Namespaces.Stanzas);
        set
        {
            if (value == null)
                RemoveTag("text", Namespaces.Stanzas);
            else
                SetTag("text", Namespaces.Stanzas, value);
        }
    }
}
