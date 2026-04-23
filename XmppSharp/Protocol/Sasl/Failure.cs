using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// This class represents a failure response in the SASL authentication process.
/// </summary>
[Tag("failure", Namespaces.Sasl)]
public class Failure : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Failure"/> class.
    /// </summary>
    public Failure() : base("failure", Namespaces.Sasl)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Failure"/> class with the specified condition and optional text.
    /// </summary>
    /// <param name="condition">The condition that caused the failure.</param>
    /// <param name="text">Optional text providing additional details about the failure.</param>
    public Failure(FailureCondition condition, string? text = default) : this()
    {
        Condition = condition;
        Text = text;
    }

    /// <summary>
    /// Gets or sets the failure condition.
    /// </summary>
    public FailureCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetMembers<FailureCondition>())
            {
                if (HasTag(name, Namespaces.Sasl))
                    return value;
            }

            return default;
        }
        set
        {
            foreach (var name in XmppEnum.GetNames<FailureCondition>())
                RemoveTag(name, Namespaces.Sasl);

            SetTag(XmppEnum.ToXml(value)!, Namespaces.Sasl);
        }
    }

    /// <summary>
    /// Gets or sets the optional text providing additional details about the failure.
    /// </summary>
    public string? Text
    {
        get => GetTag("text", Namespaces.Sasl);
        set
        {
            RemoveTag("text", Namespaces.Sasl);

            if (value != null)
                SetTag("text", Namespaces.Sasl, value);
        }
    }
}