using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppEnum]
public enum StreamErrorCondition
{
	[XmppEnumMember("bad-format")]
	BadFormat,
	
	[XmppEnumMember("bad-namespace-prefix")]
	BadNamespacePrefix,
	
	[XmppEnumMember("conflict")]
	Conflict,
	
	[XmppEnumMember("connection-timeout")]
	ConnectionTimeout,
	
	[XmppEnumMember("host-gone")]
	HostGone,
	
	[XmppEnumMember("host-unknown")]
	HostUnknown,
	
	[XmppEnumMember("improper-addressing")]
	ImproperAddressing,
	
	[XmppEnumMember("internal-server-error")]
	InternalServerError,
	
	[XmppEnumMember("invalid-from")]
	InvalidFrom,
	
	[XmppEnumMember("invalid-namespace")]
	InvalidNamespace,
	
	[XmppEnumMember("invalid-xml")]
	InvalidXml,
	
	[XmppEnumMember("not-authorized")]
	NotAuthorized,
	
	[XmppEnumMember("not-well-formed")]
	NotWellFormed,
	
	[XmppEnumMember("policy-violation")]
	PolicyViolation,
	
	[XmppEnumMember("remote-connection-failed")]
	RemoteConnectionFailed,
	
	[XmppEnumMember("reset")]
	Reset,
	
	[XmppEnumMember("resource-constraint")]
	ResourceConstraint,
	
	[XmppEnumMember("restricted-xml")]
	RestrictedXml,
	
	[XmppEnumMember("see-other-host")]
	SeeOtherHost,
	
	[XmppEnumMember("system-shutdown")]
	SystemShutdown,
	
	[XmppEnumMember("undefined-condition")]
	UndefinedCondition,
	
	[XmppEnumMember("unsupported-encoding")]
	UnsupportedEncoding,
	
	[XmppEnumMember("unsupported-feature")]
	UnsupportedFeature,
	
	[XmppEnumMember("unsupported-stanza-type")]
	UnsupportedStanzaType,
	
	[XmppEnumMember("unsupported-version")]
	UnsupportedVersion
}