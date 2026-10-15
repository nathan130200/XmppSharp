using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// Describes the defined stanza error conditions.
/// </summary>
public enum ErrorCondition
{
	/// <summary>
	/// The sender has sent a stanza containing XML that does not conform to the appropriate schema or that cannot be processed.
	/// </summary>
	[XmppEnumMember("bad-request")]
	BadRequest = 1,

	/// <summary>
	/// Access cannot be granted because an existing resource exists with the same name or address.
	/// </summary>
	[XmppEnumMember("conflict")]
	Conflict,

	/// <summary>
	/// The feature represented in the XML stanza is not implemented by the intended recipient or an intermediate server and therefore the stanza cannot be processed.
	/// </summary>
	[XmppEnumMember("feature-not-implemented")]
	FeatureNotImplemented,

	/// <summary>
	/// The requesting entity does not possess the necessary permissions to perform an action that only certain authorized roles or individuals are allowed to complete.
	/// </summary>
	[XmppEnumMember("forbidden")]
	Forbidden,

	/// <summary>
	/// The recipient or server can no longer be contacted at this address, typically on a permanent basis.
	/// </summary>
	[XmppEnumMember("gone")]
	Gone,

	/// <summary>
	/// The server has experienced a misconfiguration or other internal error that prevents it from processing the stanza.
	/// </summary>
	[XmppEnumMember("internal-server-error")]
	InternalServerError,

	/// <summary>
	/// The addressed JID or item requested cannot be found.
	/// </summary>
	[XmppEnumMember("item-not-found")]
	ItemNotFound,

	/// <summary>
	/// The sending entity has provided (e.g., during resource binding) or communicated (e.g., in the 'to' address of a stanza) an XMPP address or aspect thereof that violates the rules defined in <a href="https://xmpp.org/rfcs/rfc6122.html">XMPP Address Format</a>.
	/// </summary>
	[XmppEnumMember("jid-malformed")]
	JidMalformed,

	/// <summary>
	/// The recipient or server understands the request but cannot process it because the request does not meet criteria defined by the recipient or server.
	/// </summary>
	[XmppEnumMember("not-acceptable")]
	NotAcceptable,

	/// <summary>
	/// The recipient or server does not allow any entity to perform the action.
	/// </summary>
	[XmppEnumMember("not-allowed")]
	NotAllowed,

	/// <summary>
	/// The sender needs to provide credentials before being allowed to perform the action, or has provided improper credentials.
	/// </summary>
	[XmppEnumMember("not-authorized")]
	NotAuthorized,

	/// <summary>
	/// The entity has violated some local service policy.
	/// </summary>
	[XmppEnumMember("policy-violation")]
	PolicyViolation,

	/// <summary>
	/// The intended recipient is temporarily unavailable, undergoing maintenance, etc.
	/// </summary>
	[XmppEnumMember("recipient-unavailable")]
	RecipientUnavailable,

	/// <summary>
	/// The recipient or server is redirecting requests for this information to another entity.
	/// </summary>
	[XmppEnumMember("redirect")]
	Redirect,

	/// <summary>
	/// The requesting entity is not authorized to access the requested service because prior registration is necessary.
	/// </summary>
	[XmppEnumMember("registration-required")]
	RegistrationRequired,

	/// <summary>
	/// A remote server or service specified as part or all of the JID of the intended recipient does not exist or cannot be resolved.
	/// </summary>
	[XmppEnumMember("remote-server-not-found")]
	RemoteServerNotFound,

	/// <summary>
	/// A remote server or service specified as part or all of the JID of the intended recipient was resolved but communications could not be established within a reasonable amount of time.
	/// </summary>
	[XmppEnumMember("remote-server-timeout")]
	RemoteServerTimeout,

	/// <summary>
	/// The server or recipient is busy or lacks the system resources necessary to service the request.
	/// </summary>
	[XmppEnumMember("resource-constraint")]
	ResourceConstraint,

	/// <summary>
	/// The server or recipient does not currently provide the requested service.
	/// </summary>
	[XmppEnumMember("service-unavailable")]
	ServiceUnavailable,

	/// <summary>
	/// The requesting entity is not authorized to access the requested service because a prior subscription is necessary.
	/// </summary>
	[XmppEnumMember("subscription-required")]
	SubscriptionRequired,

	/// <summary>
	/// The error condition is not one of those defined by the other conditions in this list.
	/// </summary>
	[XmppEnumMember("undefined-condition")]
	UndefinedCondition = 0,

	/// <summary>
	/// The recipient or server understood the request but was not expecting it at this time (e.g., the request was out of order).
	/// </summary>
	[XmppEnumMember("unexpected-request")]
	UnexpectedRequest,
}