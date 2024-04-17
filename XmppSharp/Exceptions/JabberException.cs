namespace XmppSharp.Exceptions;

/// <summary>
/// Base class for extending subclasses and handling other types of error exceptions that are related to XMPP.
/// </summary>
public class JabberException : Exception
{
	/// <inheritdoc/>
	public JabberException()
	{
	}

	/// <inheritdoc/>
	public JabberException(string? message) : base(message)
	{
	}

	/// <inheritdoc/>
	public JabberException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}
