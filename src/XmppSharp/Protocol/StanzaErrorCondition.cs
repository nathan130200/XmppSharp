using System.Xml.Linq;
using XmppSharp.Utilities;

namespace XmppSharp.Protocol;

[RunStaticCtor]
public readonly struct StanzaErrorCondition : IXmppEnum<StanzaErrorCondition>
{
	private static readonly Dictionary<string, StanzaErrorCondition> s_cache = [];

	public string Value { get; init; }
	public bool HasValue => Value != null;

	StanzaErrorCondition(string value)
		=> s_cache.Add(Value = value, this);

	public static StanzaErrorCondition Parse(string value)
		=> s_cache[value];

	public static IEnumerable<StanzaErrorCondition> Values
		=> s_cache.Values;

	public XElement CreateElement()
		=> Namespaces.Stanzas.CreateElement(Value);

	public override bool Equals(object? obj)
		=> XmppEnumUtil.EqualityComparer(this, obj);

	public override int GetHashCode()
		=> Value?.GetHashCode() ?? -1;

	public static bool operator ==(StanzaErrorCondition lhs, StanzaErrorCondition rhs)
		=> lhs.Equals(rhs);

	public static bool operator !=(StanzaErrorCondition lhs, StanzaErrorCondition rhs)
		=> !(lhs == rhs);

	#region Values

	/// <summary>
	/// The sender has sent a stanza containing XML that does not conform to the appropriate schema or that cannot be processed. the associated error type SHOULD be <see cref="StanzaErrorType.Modify"/>.
	/// </summary>
	public static StanzaErrorCondition BadRequest { get; } = new("bad-request");

	/// <summary>
	/// Access cannot be granted because an existing resource exists with the same name or address; the associated error type SHOULD be <see cref="StanzaErrorType.Cancel"/>.
	/// </summary>
	public static StanzaErrorCondition Conflict { get; } = new("conflict");

	/// <summary>
	/// The feature represented in the XML stanza is not implemented by the intended recipient or an intermediate server and therefore the stanza cannot be processed. the associated error type SHOULD be <see cref="StanzaErrorType.Cancel"/> or <see cref="StanzaErrorType.Modify"/>.
	/// </summary>
	public static StanzaErrorCondition FeatureNotImplemented { get; } = new("feature-not-implemented");

	/// <summary>
	/// The requesting entity does not possess the necessary permissions to perform an action that only certain authorized roles or individuals are allowed to complete. the associated error type SHOULD be <see cref="StanzaErrorType.Auth"/>.
	/// </summary>
	public static StanzaErrorCondition Forbidden { get; } = new("forbidden");

	/// <summary>
	/// The recipient or server can no longer be contacted at this address, typically on a permanent basis. the associated error type SHOULD be <see cref="StanzaErrorType.Cancel"/> and the error stanza SHOULD include a new address (if available) as the XML character data of the <c>&lt;gone/&gt;</c> element.
	/// </summary>
	public static StanzaErrorCondition Gone { get; } = new("gone");

	/// <summary>
	/// The server has experienced a misconfiguration or other internal error that prevents it from processing the stanza; the associated error type SHOULD be <see cref="StanzaErrorType.Cancel"/>.
	/// </summary>
	public static StanzaErrorCondition InternalServerError { get; } = new("internal-server-error");

	/// <summary>
	/// The addressed JID or item requested cannot be found; the associated error type SHOULD be <see cref="StanzaErrorType.Cancel"/>.
	/// </summary>
	public static StanzaErrorCondition ItemNotFound { get; } = new("item-not-found");

	/// <summary>
	/// The sending entity has provided or communicated an XMPP address or aspect thereof that violates the rules defined in RFC-6122; the associated error type SHOULD be <see cref="StanzaErrorType.Modify"/>.
	/// </summary>
	public static StanzaErrorCondition JidMalformed { get; } = new("jid-malformed");

	/// <summary>
	/// The recipient or server understands the request but cannot process it because the request does not meet criteria defined by the recipient or server; the associated error type SHOULD be <see cref="StanzaErrorType.Modify"/>.
	/// </summary>
	public static StanzaErrorCondition NotAcceptable { get; } = new("not-acceptable");

	/// <summary>
	/// The recipient or server does not allow any entity to perform the action; the associated error type SHOULD be <see cref="StanzaErrorType.Cancel"/>.
	/// </summary>
	public static StanzaErrorCondition NotAllowed { get; } = new("not-allowed");

	/// <summary>
	/// The sender needs to provide credentials before being allowed to perform the action, or has provided improper credentials; the associated error type SHOULD be <see cref="StanzaErrorType.Auth"/>.
	/// </summary>
	public static StanzaErrorCondition NotAuthorized { get; } = new("not-authorized");

	/// <summary>
	/// The entity has violated some local service policy; the associated error type SHOULD be <see cref="StanzaErrorType.Modify"/> or <see cref="StanzaErrorType.Wait"/> depending on the policy being violated.
	/// </summary>
	public static StanzaErrorCondition PolicyViolation { get; } = new("policy-violation");

	/// <summary>
	/// The intended recipient is temporarily unavailable, undergoing maintenance, etc.; the associated error type SHOULD be <see cref="StanzaErrorType.Wait"/>.
	/// </summary>
	public static StanzaErrorCondition RecipientUnavailable { get; } = new("recipient-unavailable");

	/// <summary>
	/// The recipient or server is redirecting requests for this information to another entity, typically in a temporary fashion; the associated error type SHOULD be <see cref="StanzaErrorType.Modify"/> and the error stanza SHOULD contain the alternate address in the XML character data of the element content.
	/// </summary>
	public static StanzaErrorCondition Redirect { get; } = new("redirect");

	/// <summary>
	/// The requesting entity is not authorized to access the requested service because prior registration is necessary; the associated error type SHOULD be <see cref="StanzaErrorType.Wait"/>.
	/// </summary>
	public static StanzaErrorCondition RegistrationRequired { get; } = new("registration-required");

	/// <summary>
	/// A remote server or service specified as part or all of the JID of the intended recipient does not exist or cannot be resolved; the associated error type SHOULD be <see cref="StanzaErrorType.Cancel"/>.
	/// </summary>
	public static StanzaErrorCondition RemoteServerNotFound { get; } = new("remote-server-not-found");

	/// <summary>
	/// A remote server or service specified as part or all of the JID of the intended recipient (or needed to fulfill a request) was resolved but communications could not be established within a reasonable amount of time; the associated error type SHOULD be <see cref="StanzaErrorType.Wait"/> (unless the error is of a more permanent nature, e.g., the remote server is found but it cannot be authenticated or it violates security policies).
	/// </summary>
	public static StanzaErrorCondition RemoteServerTimeout { get; } = new("remote-server-timeout");

	/// <summary>
	/// The server or recipient is busy or lacks the system resources necessary to service the request; the associated error type SHOULD be <see cref="StanzaErrorType.Wait"/>.
	/// </summary>
	public static StanzaErrorCondition ResourceConstraint { get; } = new("resource-constraint");

	/// <summary>
	/// The server or recipient does not currently provide the requested service; the associated error type SHOULD be <see cref="StanzaErrorType.Cancel"/>.
	/// </summary>
	public static StanzaErrorCondition ServiceUnavailable { get; } = new("service-unavailable");

	/// <summary>
	/// The requesting entity is not authorized to access the requested service because a prior subscription is necessary; the associated error type SHOULD be <see cref="StanzaErrorType.Auth"/>.
	/// </summary>
	public static StanzaErrorCondition SubscriptionRequired { get; } = new("subscription-required");

	/// <summary>
	/// The recipient or server understood the request but was not expecting it at this time (e.g., the request was out of order); the associated error type SHOULD be <see cref="StanzaErrorType.Wait"/> or <see cref="StanzaErrorType.Modify"/>.
	/// </summary>
	public static StanzaErrorCondition UnexpectedRequest { get; } = new("unexpected-request");

	/// <summary>
	/// The error condition is not one of those defined by the other conditions in this list; any error type can be associated with this condition, and it SHOULD NOT be used except in conjunction with an application-specific condition.
	/// </summary>
	public static StanzaErrorCondition UndefinedCondition { get; } = new("undefined-condition");

	#endregion
}
