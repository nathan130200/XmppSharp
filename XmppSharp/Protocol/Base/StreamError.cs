using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents an error element used within XMPP streams to signal errors during communication establishment.
/// <para>This class provides properties to specify the error condition and an optional text description for more detailed explanations.</para>
/// </summary>
[XmppTag("error", Namespaces.Stream)]
public class StreamError : Element
{
    // <summary>
    /// Initializes a new instance of the <see cref="StreamError"/> class.
    /// </summary>
    public StreamError() : base("stream:error", Namespaces.Stream)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamError"/> class with the specified error condition.
    /// </summary>
    /// <param name="condition">The XMPP stream error condition.</param>
    public StreamError(StreamErrorCondition condition) : this()
    {
        Condition = condition;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamError"/> class with the specified error condition and text description.
    /// </summary>
    /// <param name="condition">The XMPP stream error condition.</param>
    /// <param name="text">An optional text description of the error.</param>
    public StreamError(StreamErrorCondition condition, string? text) : this(condition)
    {
        Text = text;
    }

    /// <summary>
    /// Gets or sets the error condition associated with the stream error, indicating the specific issue encountered.
    /// </summary>
    public StreamErrorCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetValues<StreamErrorCondition>())
            {
                if (HasTag(name, Namespaces.Streams))
                    return value;
            }

            return StreamErrorCondition.UndefinedCondition;
        }
        set
        {
            foreach (var tag in XmppEnum.GetNames<StreamErrorCondition>())
                RemoveTag(tag, Namespaces.Streams);

            SetTag(XmppEnum.ToXmppName(value), xmlns: Namespaces.Streams);
        }
    }

    /// <summary>
    /// Gets or sets the optional text description of the stream error, providing additional details about the issue.
    /// </summary>
    public string? Text
    {
        get => GetTag("text", Namespaces.Streams);
        set
        {
            if (value == null)
                RemoveTag("text", Namespaces.Streams);
            else
                SetTag("text", Namespaces.Streams, value);
        }
    }
}