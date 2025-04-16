using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppEnum]
public enum StanzaErrorCondition
{
    Unspecified = -1,

    [XmppMember("bad-request")]
    BadRequest,

    [XmppMember("conflict")]
    Conflict,

    [XmppMember("feature-not-implemented")]
    FeatureNotImplemented,

    [XmppMember("forbidden")]
    Forbidden,

    [XmppMember("gone")]
    Gone,

    [XmppMember("internal-server-error")]
    InternalServerError,

    [XmppMember("item-not-found")]
    ItemNotFound,

    [XmppMember("jid-malformed")]
    JidMalformed,

    [XmppMember("not-acceptable")]
    NotAcceptable,

    [XmppMember("not-allowed")]
    NotAllowed,

    [XmppMember("not-authorized")]
    NotAuthorized,

    [XmppMember("payment-required")]
    PaymentRequired,

    [XmppMember("policy-violation")]
    PolicyViolation,

    [XmppMember("recipient-unavailable")]
    RecipientUnavailable,

    [XmppMember("redirect")]
    Redirect,

    [XmppMember("registration-required")]
    RegistrationRequired,

    [XmppMember("remote-server-not-found")]
    RemoteServerNotFound,

    [XmppMember("remote-server-timeout")]
    RemoteServerTimeout,

    [XmppMember("resource-constraint")]
    ResourceConstraint,

    [XmppMember("service-unavailable")]
    ServiceUnavailable,

    [XmppMember("subscription-required")]
    SubscriptionRequired,

    [XmppMember("undefined-condition")]
    UndefinedCondition,

    [XmppMember("unexpected-request")]
    UnexpectedRequest
}
