using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppEnum]
public enum FailureCondition
{
    [XmppEnumMember("aborted")]
    Aborted,

    [XmppEnumMember("account-disabled")]
    AccountDisabled,

    [XmppEnumMember("credentials-expired")]
    CredentialsExpired,

    [XmppEnumMember("encryption-required")]
    EncryptionRequired,

    [XmppEnumMember("incorrect-encoding")]
    IncorrectEncoding,

    [XmppEnumMember("invalid-authzid")]
    InvalidAuthzid,

    [XmppEnumMember("invalid-mechanism")]
    InvalidMechanism,

    [XmppEnumMember("malformed-request")]
    MalformedRequest,

    [XmppEnumMember("mechanism-too-weak")]
    MechanismTooWeak,

    [XmppEnumMember("not-authorized")]
    NotAuthorized,

    [XmppEnumMember("temporary-auth-failure")]
    TemporaryAuthFailure,
}