using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Defines the error conditions that can be used in XMPP stanzas to indicate specific error types.
/// </summary>
public enum ErrorCondition
{
    /// <summary>
    /// Indicates that the request was malformed or contained invalid syntax. This error condition is used when the server cannot process the request due to a client error.
    /// </summary>
    [XmppEnumMember("bad-request")]
    BadRequest = 1,

    /// <summary>
    /// Indicates that the request could not be processed because it would conflict with the current state of the server or resource.
    /// </summary>
    [XmppEnumMember("conflict")]
    Conflict,

    /// <summary>
    /// Indicates that the requested feature is not implemented on the server.
    /// </summary>
    [XmppEnumMember("feature-not-implemented")]
    FeatureNotImplemented,

    /// <summary>
    /// Indicates that the action is forbidden and the user does not have the necessary permissions.
    /// </summary>
    [XmppEnumMember("forbidden")]
    Forbidden,

    /// <summary>
    /// Indicates that the requested resource is no longer available at the server.
    /// </summary>
    [XmppEnumMember("gone")]
    Gone,

    /// <summary>
    /// Indicates that an internal server error has occurred.
    /// </summary>
    [XmppEnumMember("internal-server-error")]
    InternalServerError,

    /// <summary>
    /// Indicates that the requested item could not be found.
    /// </summary>
    [XmppEnumMember("item-not-found")]
    ItemNotFound,

    /// <summary>
    /// Indicates that the JID (Jabber ID) provided is malformed.
    /// </summary>
    [XmppEnumMember("jid-malformed")]
    JidMalformed,

    /// <summary>
    /// Indicates that the request is not acceptable according to the server's policies.
    /// </summary>
    [XmppEnumMember("not-acceptable")]
    NotAcceptable,

    /// <summary>
    /// Indicates that the action is not allowed.
    /// </summary>
    [XmppEnumMember("not-allowed")]
    NotAllowed,

    /// <summary>
    /// Indicates that the user is not authorized to perform the requested action.
    /// </summary>
    [XmppEnumMember("not-authorized")]
    NotAuthorized,

    /// <summary>
    /// Indicates that payment is required to perform the requested action.
    /// </summary>
    [XmppEnumMember("payment-required")]
    PaymentRequired,

    /// <summary>
    /// Indicates that the action violates the server's policies.
    /// </summary>
    [XmppEnumMember("policy-violation")]
    PolicyViolation,

    /// <summary>
    /// Indicates that the recipient is unavailable.
    /// </summary>
    [XmppEnumMember("recipient-unavailable")]
    RecipientUnavailable,

    /// <summary>
    /// Indicates that the request should be redirected to another entity.
    /// </summary>
    [XmppEnumMember("redirect")]
    Redirect,

    /// <summary>
    /// Indicates that registration is required to perform the requested action.
    /// </summary>
    [XmppEnumMember("registration-required")]
    RegistrationRequired,

    /// <summary>
    /// Indicates that the remote server could not be found.
    /// </summary>
    [XmppEnumMember("remote-server-not-found")]
    RemoteServerNotFound,

    /// <summary>
    /// Indicates that the remote server did not respond within the expected time frame.
    /// </summary>
    [XmppEnumMember("remote-server-timeout")]
    RemoteServerTimeout,

    /// <summary>
    /// Indicates that the server is experiencing resource constraints and cannot process the request.
    /// </summary>
    [XmppEnumMember("resource-constraint")]
    ResourceConstraint,

    /// <summary>
    /// Indicates that the service is unavailable.
    /// </summary>
    [XmppEnumMember("service-unavailable")]
    ServiceUnavailable,

    /// <summary>
    /// Indicates that a subscription is required to perform the requested action.
    /// </summary>
    [XmppEnumMember("subscription-required")]
    SubscriptionRequired,

    /// <summary>
    /// Indicates that the error condition is undefined.
    /// </summary>
    [XmppEnumMember("undefined-condition")]
    UndefinedCondition,

    /// <summary>
    /// Indicates that the request was unexpected.
    /// </summary>
    [XmppEnumMember("unexpected-request")]
    UnexpectedRequest
}
