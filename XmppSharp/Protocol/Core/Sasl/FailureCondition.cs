using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Core.Sasl;

[XmppEnum]
public enum FailureCondition
{
    [XmppMember("aborted")]
    Aborted,

    [XmppMember("account-disabled")]
    AccountDisabled,

    [XmppMember("credentials-expired")]
    CredentialsExpired,

    [XmppMember("encryption-required")]
    EncryptionRequired,

    [XmppMember("incorrect-encoding")]
    IncorrectEncoding,

    [XmppMember("invalid-authzid")]
    InvalidAuthzid,

    [XmppMember("invalid-mechanism")]
    InvalidMechanism,

    [XmppMember("malformed-request")]
    MalformedRequest,

    [XmppMember("mechanism-too-weak")]
    MechanismTooWeak,

    [XmppMember("not-authorized")]
    NotAuthorized,

    [XmppMember("temporary-auth-failure")]
    TemporaryAuthFailure
}
