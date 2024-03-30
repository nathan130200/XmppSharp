using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Represents a "failure" element used in Simple Authentication and Security Layer (SASL) negotiation within XMPP.
/// </summary>
[XmppTag("failure", Namespaces.Sasl)]
public class Failure : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Failure"/> class with default properties.
    /// </summary>
    public Failure() : base("failure", Namespaces.Sasl)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Failure"/> class with the specified failure condition.
    /// </summary>
    /// <param name="condition">The condition that caused the authentication failure.</param>
    public Failure(FailureCondition condition) : this()
    {
        Condition = condition;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Failure"/> class with the specified failure condition and explanatory text.
    /// </summary>
    /// <param name="condition">The condition that caused the authentication failure.</param>
    /// <param name="text">Additional text providing more information about the failure.</param>
    public Failure(FailureCondition condition, string text) : this(condition)
    {
        Text = text;
    }

    /// <summary>
    /// Gets or sets the failure condition that caused the authentication failure.
    /// </summary>
    public FailureCondition? Condition
    {
        get
        {
            foreach (var (key, value) in XmppEnum.GetValues<FailureCondition>())
            {
                if (HasTag(key))
                    return value;
            }

            return default;
        }
        set
        {
            if (Condition.TryGetValue(out var oldValue))
                RemoveTag(XmppEnum.ToXmppName(oldValue));

            if (value.TryGetValue(out var result))
                SetTag(XmppEnum.ToXmppName(result));
        }
    }

    /// <summary>
    /// Gets or sets additional text providing more information about the authentication failure.
    /// </summary>
    public string? Text
    {
        get => GetTag("text", Namespaces.Sasl);
        set
        {
            if (value == null)
                RemoveTag("text", Namespaces.Sasl);
            else
                SetTag("text", Namespaces.Sasl, value);
        }
    }
}
