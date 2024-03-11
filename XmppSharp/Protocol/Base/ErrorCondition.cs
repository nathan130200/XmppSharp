using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppEnum]
public enum ErrorCondition
{
    [XmppEnumMember("bad-request")]
    BadRequest,

    [XmppEnumMember("conflict")]
    Conflict,

    [XmppEnumMember("feature-not-implemented")]
    FeatureNotImplemented,

    [XmppEnumMember("forbidden")]
    Forbidden,

    [XmppEnumMember("gone")]
    Gone,

    [XmppEnumMember("internal-server-error")]
    InternalServerError,

    [XmppEnumMember("item-not-found")]
    ItemNotFound,

    [XmppEnumMember("jid-malformed")]
    JidMalformed,

    [XmppEnumMember("not-acceptable")]
    NotAcceptable,

    [XmppEnumMember("not-allowed")]
    NotAllowed,

    [XmppEnumMember("not-authorized")]
    NotAuthorized,

    [XmppEnumMember("policy-violation")]
    PolicyViolation,

    [XmppEnumMember("recipient-unavailable")]
    RecipientUnavailable,

    [XmppEnumMember("redirect")]
    Redirect,

    [XmppEnumMember("registration-required")]
    RegistrationRequired,

    [XmppEnumMember("remote-server-not-found")]
    RemoteServerNotFound,

    [XmppEnumMember("remote-server-timeout")]
    RemoteServerTimeout,

    [XmppEnumMember("resource-constraint")]
    ResourceConstraint,

    [XmppEnumMember("service-unavailable")]
    ServiceUnavailable,

    [XmppEnumMember("subscription-required")]
    SubscriptionRequired,

    [XmppEnumMember("undefined-condition")]
    UndefinedCondition,

    [XmppEnumMember("unexpected-request")]
    UnexpectedRequest,
}