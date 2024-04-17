using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Defines a collection of error conditions that can occur during XMPP stream communication.
/// </summary>
[XmppEnum]
public enum StreamErrorCondition
{
	/// <summary>
	/// Indicates a malformed XML format in the received data.
	/// </summary>
	[XmppMember("bad-format")]
	BadFormat,

	/// <summary>
	/// Signals an invalid namespace prefix used in the communication.
	/// </summary>
	[XmppMember("bad-namespace-prefix")]
	BadNamespacePrefix,

	/// <summary>
	/// Represents a resource naming conflict.
	/// </summary>
	[XmppMember("conflict")]
	Conflict,

	/// <summary>
	/// Indicates a connection timeout during communication establishment.
	/// </summary>
	[XmppMember("connection-timeout")]
	ConnectionTimeout,

	/// <summary>
	/// Signals that the target host is no longer available.
	/// </summary>
	[XmppMember("host-gone")]
	HostGone,

	/// <summary>
	/// Indicates that the specified host cannot be found.
	/// </summary>
	[XmppMember("host-unknown")]
	HostUnknown,

	/// <summary>
	/// Represents an issue with message addressing within an XMPP stanza.
	/// </summary>
	[XmppMember("improper-addressing")]
	ImproperAddressing,

	/// <summary>
	/// Signals a generic server-side error.
	/// </summary>
	[XmppMember("internal-server-error")]
	InternalServerError,

	/// <summary>
	/// Indicates an invalid "from" attribute value in an XMPP stanza.
	/// </summary>
	[XmppMember("invalid-from")]
	InvalidFrom,

	/// <summary>
	/// Signals an invalid namespace used in the communication.
	/// </summary>
	[XmppMember("invalid-namespace")]
	InvalidNamespace,

	/// <summary>
	/// Indicates invalid XML content within the received data.
	/// </summary>
	[XmppMember("invalid-xml")]
	InvalidXml,

	/// <summary>
	/// Signals that the provided credentials are not authorized for the requested action.
	/// </summary>
	[XmppMember("not-authorized")]
	NotAuthorized,

	/// <summary>
	/// Indicates a malformed XML structure in the received data.
	/// </summary>
	[XmppMember("not-well-formed")]
	NotWellFormed,

	/// <summary>
	/// Signals a violation of a server policy by the client.
	/// </summary>
	[XmppMember("policy-violation")]
	PolicyViolation,

	/// <summary>
	/// Indicates a failure to establish a connection to a remote server.
	/// </summary>
	[XmppMember("remote-connection-failed")]
	RemoteConnectionFailed,

	/// <summary>
	/// Signals a server reset requiring reconnection.
	/// </summary>
	[XmppMember("reset")]
	Reset,

	/// <summary>
	/// Indicates insufficient server resources to handle the request.
	/// </summary>
	[XmppMember("resource-constraint")]
	ResourceConstraint,

	/// <summary>
	/// Signals that the XML content violates server restrictions.
	/// </summary>
	[XmppMember("restricted-xml")]
	RestrictedXml,

	/// <summary>
	/// Instructs the client to connect to a different host.
	/// </summary>
	[XmppMember("see-other-host")]
	SeeOtherHost,

	/// <summary>
	/// Signals a server shutdown in progress.
	/// </summary>
	[XmppMember("system-shutdown")]
	SystemShutdown,

	/// <summary>
	/// Represents an unspecified error condition.
	/// </summary>
	[XmppMember("undefined-condition")]
	UndefinedCondition,

	/// <summary>
	/// Signals that the server doesn't support the client's character encoding.
	/// </summary>
	[XmppMember("unsupported-encoding")]
	UnsupportedEncoding,

	/// <summary>
	/// Signals that the server doesn't support a requested feature.
	/// </summary>
	[XmppMember("unsupported-feature")]
	UnsupportedFeature,

	/// <summary>
	/// Signals that the server doesn't support a requested stanza.
	/// </summary>
	[XmppMember("unsupported-stanza-type")]
	UnsupportedStanzaType,

	/// <summary>
	/// Signals that the server doesn't support a requested version.
	/// </summary>
	[XmppMember("unsupported-version")]
	UnsupportedVersion
}