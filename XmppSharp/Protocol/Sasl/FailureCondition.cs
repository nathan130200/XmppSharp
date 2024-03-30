using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Enumerates potential failure conditions that can occur during Simple Authentication and Security Layer (SASL) negotiation within XMPP.
/// </summary>
[XmppEnum]
public enum FailureCondition
{
    /// <summary>
    /// Indicates that the authentication process was aborted before completion, often due to external factors or client-side decisions.
    /// </summary>
    [XmppMember("aborted")]
    Aborted,

    /// <summary>
    /// Indicates that the account being used for authentication is currently disabled on the server.
    /// </summary>
    [XmppMember("account-disabled")]
    AccountDisabled,

    /// <summary>
    /// Indicates that the provided credentials (username and/or password) have expired and are no longer valid.
    /// </summary>
    [XmppMember("credentials-expired")]
    CredentialsExpired,

    /// <summary>
    /// Indicates that the server requires encrypted communication before proceeding with authentication.
    /// <para>The client should establish a secure connection and initiate SASL negotiation again.</para>
    /// </summary>
    [XmppMember("encryption-required")]
    EncryptionRequired,

    /// <summary>
    /// Indicates that the client's response was not encoded correctly according to the SASL mechanism's specifications.
    /// </summary>
    [XmppMember("incorrect-encoding")]
    IncorrectEncoding,

    /// <summary>
    /// Indicates that the provided authorization identity (authzid) is invalid or not allowed for the user.
    /// </summary>
    [XmppMember("invalid-authzid")]
    InvalidAuthzid,

    /// <summary>
    /// Indicates that the selected SASL mechanism is not supported or allowed by the server.
    /// </summary>
    [XmppMember("invalid-mechanism")]
    InvalidMechanism,

    /// <summary>
    /// Indicates that the client's authentication request was malformed or did not adhere to the protocol's format.
    /// </summary>
    [XmppMember("malformed-request")]
    MalformedRequest,

    /// <summary>
    /// Indicates that the selected SASL mechanism is considered too weak or has known vulnerabilities.
    /// <para>The client should choose a stronger mechanism.</para>
    /// </summary>
    [XmppMember("mechanism-too-weak")]
    MechanismTooWeak,

    /// <summary>
    /// Indicates that the provided credentials are not authorized to access the requested service or resource.
    /// </summary>
    [XmppMember("not-authorized")]
    NotAuthorized,

    /// <summary>
    /// Indicates a temporary authentication failure, often due to external systems or temporary server-side issues.
    /// <para>The client should retry the authentication process later.</para>
    /// </summary>
    [XmppMember("temporary-auth-failure")]
    TemporaryAuthFailure
}