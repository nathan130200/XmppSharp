using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents the various error conditions that can be signaled in an XMPP stanza error element.
/// </summary>
[XmppEnum]
public enum StanzaErrorCondition
{
	/// <summary>
	/// The request was invalid.
	/// </summary>
	[XmppMember("bad-request")]
	BadRequest,

	/// <summary>
	/// The operation failed due to a conflict (e.g., resource already exists).
	/// </summary>
	[XmppMember("conflict")]
	Conflict,

	/// <summary>
	/// The requested feature is not implemented on the server.
	/// </summary>
	[XmppMember("feature-not-implemented")]
	FeatureNotImplemented,

	/// <summary>
	/// The sender is not authorized to perform the requested operation.
	/// </summary>
	[XmppMember("forbidden")]
	Forbidden,

	/// <summary>
	/// The requested entity (e.g., user, room) no longer exists.
	/// </summary>
	[XmppMember("gone")]
	Gone,

	/// <summary>
	/// The server encountered an internal error and could not process the request.
	/// </summary>
	[XmppMember("internal-server-error")]
	InternalServerError,

	/// <summary>
	/// The requested item (e.g., message, roster entry) was not found.
	/// </summary>
	[XmppMember("item-not-found")]
	ItemNotFound,

	/// <summary>
	/// The JID (Jabber ID) specified in the request was invalid or malformed.
	/// </summary>
	[XmppMember("jid-malformed")]
	JidMalformed,

	/// <summary>
	/// The server cannot accept the request with the provided parameters.
	/// </summary>
	[XmppMember("not-acceptable")]
	NotAcceptable,

	/// <summary>
	/// The requested operation is not allowed for the current resource or context.
	/// </summary>
	[XmppMember("not-allowed")]
	NotAllowed,

	/// <summary>
	/// The sender is not authorized to access the requested resource.
	/// </summary>
	[XmppMember("not-authorized")]
	NotAuthorized,

	/// <summary>
	/// The request violates server policy.
	/// </summary>
	[XmppMember("policy-violation")]
	PolicyViolation,

	/// <summary>
	/// The intended recipient is currently unavailable.
	/// </summary>
	[XmppMember("recipient-unavailable")]
	RecipientUnavailable,

	/// <summary>
	/// The server is redirecting the client to a different location.
	/// </summary>
	[XmppMember("redirect")]
	Redirect,

	/// <summary>
	/// The client needs to register with the server before performing the request.
	/// </summary>
	[XmppMember("registration-required")]
	RegistrationRequired,

	/// <summary>
	/// The server could not locate the remote server specified in the request.
	/// </summary>
	[XmppMember("remote-server-not-found")]
	RemoteServerNotFound,

	/// <summary>
	/// The server timed out waiting for a response from the remote server.
	/// </summary>
	[XmppMember("remote-server-timeout")]
	RemoteServerTimeout,

	/// <summary>
	/// The server lacks the resources to process the request.
	/// </summary>
	[XmppMember("resource-constraint")]
	ResourceConstraint,

	/// <summary>
	/// The requested service is unavailable on the server.
	/// </summary>
	[XmppMember("service-unavailable")]
	ServiceUnavailable,

	/// <summary>
	/// The client needs to subscribe or be authorized before performing the request.
	/// </summary>
	[XmppMember("subscription-required")]
	SubscriptionRequired,

	/// <summary>
	/// The error condition is not explicitly defined in the XMPP standard.
	/// </summary>
	[XmppMember("undefined-condition")]
	UndefinedCondition,

	/// <summary>
	/// The server received an unexpected request type.
	/// </summary>
	[XmppMember("unexpected-request")]
	UnexpectedRequest
}