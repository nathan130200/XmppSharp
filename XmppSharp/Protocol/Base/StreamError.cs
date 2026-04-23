using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents an error that occurred during the stream negotiation phase of an XMPP connection.
/// </summary>
/// <remarks>
/// This class encapsulates the details of the stream error, including the specific condition that caused the error and any additional text information provided by the server.
/// </remarks>
[Tag("error", Namespaces.Stream)]
[Tag("stream:error", Namespaces.Stream)]
public class StreamError : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamError"/> class.
    /// </summary>
    public StreamError() : base("stream:error", Namespaces.Stream)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamError"/> class with the specified error condition.
    /// </summary>
    /// <param name="condition">
    /// The specific condition that caused the stream error.
    /// </param>
    public StreamError(StreamErrorCondition condition) : this()
    {
        Condition = condition;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamError"/> class with the specified error condition and additional text information.
    /// </summary>
    /// <param name="condition">
    /// The specific condition that caused the stream error.
    /// </param>
    /// <param name="text">
    /// Additional text information providing more details about the stream error.
    /// </param>
    public StreamError(StreamErrorCondition condition, string? text) : this(condition)
    {
        Text = text;
    }

    /// <summary>
    /// Gets or sets the specific condition that caused the stream error.
    /// </summary>
    public StreamErrorCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetMembers<StreamErrorCondition>())
            {
                if (HasTag(name, Namespaces.Streams))
                    return value;
            }

            return StreamErrorCondition.UndefinedCondition;
        }
        set
        {
            foreach (var name in XmppEnum.GetNames<StreamErrorCondition>())
                RemoveTag(name, Namespaces.Streams);

            if (Enum.IsDefined(value))
                SetTag(value.ToXml()!, Namespaces.Streams);
        }
    }

    /// <summary>
    /// Gets or sets additional text information providing more details about the stream error.
    /// </summary>
    public string? Text
    {
        get => GetTag("text", Namespaces.Streams);
        set
        {
            RemoveTag("text", Namespaces.Streams);

            if (value != null)
                SetTag("text", Namespaces.Streams, value);
        }
    }
}
