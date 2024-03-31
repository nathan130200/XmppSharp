namespace XmppSharp;

/// <summary>
/// Represents an error that occurred in the XMPP enum.
/// </summary>
public class XmppEnumException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="XmppEnumException" />.
    /// </summary>
    public XmppEnumException() : base()
    {

    }

    /// <summary>
    /// Initializes a new instance of <see cref="XmppEnumException" />.
    /// </summary>
    /// <param name="message">Description of the problem about what may have caused it.</param>
    public XmppEnumException(string? message) : base(message)
    {

    }
}
