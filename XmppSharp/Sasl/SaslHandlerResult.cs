using XmppSharp.Exceptions;
using XmppSharp.Protocol.Sasl;

namespace XmppSharp.Sasl;

/// <summary>
/// Represents the result of a SASL (Simple Authentication and Security Layer) operation.
/// </summary>
/// <remarks>
/// This struct encapsulates the outcome of a SASL operation, including its type and any associated exception.
/// </remarks>
public readonly struct SaslHandlerResult
{
    public XmppSaslHandlerResultType Type { get; init; }
    public Exception Exception { get; init; }

    /// <summary>
    /// Gets a result indicating that the SASL process should continue with further connection processing.
    /// </summary>
    public static SaslHandlerResult Continue => new() { Type = XmppSaslHandlerResultType.Continue };

    /// <summary>
    /// Gets a result indicating a successful SASL authentication operation.
    /// </summary>
    public static SaslHandlerResult Success => new() { Type = XmppSaslHandlerResultType.Success };

    /// <summary>
    /// Creates a result indicating a failure in the SASL authentication process.
    /// </summary>
    /// <param name="condition">The specific failure condition that caused the authentication to fail.</param>
    /// <param name="text">An optional descriptive message providing additional context about the failure. This value can be <see
    /// langword="null"/> if no additional information is available.</param>
    /// <returns>A <see cref="SaslHandlerResult"/> representing the failure, with the result type set to  <see
    /// cref="XmppSaslHandlerResultType.Error"/> and an associated exception describing the failure.</returns>
    public static SaslHandlerResult Failure(FailureCondition condition, string? text = default) => new()
    {
        Type = XmppSaslHandlerResultType.Error,
        Exception = new JabberSaslException(condition, text)
    };

    /// <summary>
    /// Creates a result representing an error that occurred during SASL handling.
    /// </summary>
    /// <param name="ex">The exception that describes the error. Cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="SaslHandlerResult"/> instance with its type set to <see cref="XmppSaslHandlerResultType.Error"/> and
    /// the provided exception.</returns>
    public static SaslHandlerResult Error(Exception ex) => new()
    {
        Type = XmppSaslHandlerResultType.Error,
        Exception = ex
    };
}
