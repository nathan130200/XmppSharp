using XmppSharp.Protocol.Base;

namespace XmppSharp.Exceptions;

/// <summary>
/// Represents errors that can occur with the XMPP stream while parsing.
/// </summary>
public sealed class JabberStreamException : JabberException
{
    /// <summary>
    /// Determines the condition of the problem.
    /// </summary>
    public StreamErrorCondition Condition { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="JabberStreamException" /> with the specified condition.
    /// </summary>
    /// <param name="condition">Error condition that caused the exception.</param>
    public JabberStreamException(StreamErrorCondition condition) : base($"XMPP stream error of type '{condition}' was thrown.")
        => Condition = condition;

    /// <summary>
    /// Initializes a new instance of <see cref="JabberStreamException" /> with the specified condition and inner exception.
    /// </summary>
    /// <param name="condition">Error condition that caused the exception.</param>
    /// <param name="innerException">Inner exception that may have caused the problem.</param>
    public JabberStreamException(StreamErrorCondition condition, Exception innerException) : base($"XMPP stream error of type '{condition}' was thrown.", innerException)
        => Condition = condition;

    public JabberStreamException(StreamErrorCondition condition, string message) : base(message)
        => Condition = condition;
}
