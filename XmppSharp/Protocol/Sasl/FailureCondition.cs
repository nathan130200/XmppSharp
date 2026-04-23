using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Defines the failure conditions for SASL authentication.
/// </summary>
public enum FailureCondition
{
    /// <summary>
    /// Indicates that the authentication process was aborted by the client or server.
    /// </summary>
    [XmppEnumMember("aborted")]
    Aborted = 1,

    /// <summary>
    /// Indicates that the authentication process failed due to an invalid authentication identity (authcid).
    /// </summary>
    [XmppEnumMember("account-disabled")]
    AccountDisabled,

    /// <summary>
    /// Indicates that the authentication process failed because the credentials provided have expired.
    /// </summary>
    [XmppEnumMember("credentials-expired")]
    CredentialsExpired,

    /// <summary>
    /// Indicates that the authentication process failed because the encryption requirements were not met.
    /// </summary>
    [XmppEnumMember("encryption-required")]
    EncryptionRequired,

    /// <summary>
    /// Indicates that the authentication process failed due to an incorrect encoding of the authentication data.
    /// </summary>
    [XmppEnumMember("incorrect-encoding")]
    IncorrectEncoding,

    /// <summary>
    /// Indicates that the authentication process failed due to an invalid authorization identity (authzid).
    /// </summary>
    [XmppEnumMember("invalid-authzid")]
    InvalidAuthzid,

    /// <summary>
    /// Indicates that the authentication process failed due to an invalid mechanism.
    /// </summary>
    [XmppEnumMember("invalid-mechanism")]
    InvalidMechanism,

    /// <summary>
    /// Indicates that the authentication process failed due to a malformed request.
    /// </summary>
    [XmppEnumMember("malformed-request")]
    MalformedRequest,

    /// <summary>
    /// Indicates that the authentication process failed because the mechanism used is too weak.
    /// </summary>
    [XmppEnumMember("mechanism-too-weak")]
    MechanismTooWeak,

    /// <summary>
    /// Indicates that the authentication process failed because the client is not authorized.
    /// </summary>
    [XmppEnumMember("not-authorized")]
    NotAuthorized,

    /// <summary>
    /// Indicates that the authentication process failed due to a temporary authentication failure.
    /// </summary>
    [XmppEnumMember("temporary-auth-failure")]
    TemporaryAuthFailure
}
