namespace XmppSharp.Sasl;

/// <summary>
/// Represents the result type of an XMPP SASL (Simple Authentication and Security Layer) handler operation.
/// </summary>
public enum XmppSaslHandlerResultType
{
    /// <summary>
    /// Represents a placeholder or undefined value.
    /// </summary>
    None,

    /// <summary>
    /// Represents the continuation of a process or operation.
    /// </summary>
    Continue,

    // auth success response

    /// <summary>
    /// Represents a successful authentication response.
    /// </summary>
    Success,

    /// <summary>
    /// Represents an error that occurred during SASL (Simple Authentication and Security Layer) handling.
    /// </summary>
    Error
}