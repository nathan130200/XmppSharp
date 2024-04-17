using XmppSharp.Protocol.Sasl;

namespace XmppSharp.Exceptions;

/// <summary>
/// Represents the error that occurred during SASL authentication.
/// </summary>
public sealed class JabberSaslException : JabberException
{
	/// <summary>
	/// Determines the condition of the problem.
	/// </summary>
	public FailureCondition Condition { get; }

	/// <summary>
	/// Initializes a new instance of <see cref="JabberSaslException" /> with the specified condition.
	/// </summary>
	/// <param name="condition">Error condition that caused the exception.</param>
	public JabberSaslException(FailureCondition condition) : base($"SASL authentication failed with error {condition}.")
		=> this.Condition = condition;

	/// <summary>
	/// Initializes a new instance of <see cref="JabberSaslException" /> with the specified condition and inner exception.
	/// </summary>
	/// <param name="condition">Error condition that caused the exception.</param>
	/// <param name="innerException">Inner exception that may have caused the problem.</param>
	public JabberSaslException(FailureCondition condition, Exception innerException) : base($"SASL authentication failed with error {condition}.", innerException)
	{

	}
}