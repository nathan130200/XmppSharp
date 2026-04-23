using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Defines the stream error conditions as per XMPP specifications.
/// </summary>
public enum StreamErrorCondition
{
    /// <summary>
    /// Indicates that the XML is not well-formed or does not conform to the expected structure.
    /// </summary>
    [XmppEnumMember("bad-format")]
    BadFormat = 1,

    /// <summary>
    /// Indicates that the namespace prefix is not properly defined or is not allowed in the context of the stream.
    /// </summary>
    [XmppEnumMember("bad-namespace-prefix")]
    BadNamespacePrefix,

    /// <summary>
    /// Indicates that the stream is being closed due to a conflict, such as a conflict in resource binding or authentication.
    /// </summary>
    [XmppEnumMember("conflict")]
    Conflict,

    /// <summary>
    /// Indicates that the connection has been idle for too long and is being closed by the server due to a timeout.
    /// </summary>
    [XmppEnumMember("connection-timeout")]
    ConnectionTimeout,

    /// <summary>
    /// Indicates that the host or server is no longer available, either because it has gone offline or because it is unreachable.
    /// </summary>
    [XmppEnumMember("host-gone")]
    HostGone,

    /// <summary>
    /// Indicates that the host or server is unknown, either because it does not exist or because it cannot be found by the client.
    /// </summary>
    [XmppEnumMember("host-unknown")]
    HostUnknown,

    /// <summary>
    /// Indicates that the addressing of the stanza is improper or invalid.
    /// </summary>
    [XmppEnumMember("improper-addressing")]
    ImproperAddressing,

    /// <summary>
    /// Indicates that an internal server error has occurred.
    /// </summary>
    [XmppEnumMember("internal-server-error")]
    InternalServerError,

    /// <summary>
    /// Indicates that the "from" attribute in the stanza is invalid.
    /// </summary>
    [XmppEnumMember("invalid-from")]
    InvalidFrom,

    /// <summary>
    /// Indicates that the namespace in the stanza is invalid.
    /// </summary>
    [XmppEnumMember("invalid-namespace")]
    InvalidNamespace,

    /// <summary>
    /// Indicates that the XML in the stanza is invalid.
    /// </summary>
    [XmppEnumMember("invalid-xml")]
    InvalidXml,

    /// <summary>
    /// Indicates that the entity is not authorized to perform the requested action.
    /// </summary>
    [XmppEnumMember("not-authorized")]
    NotAuthorized,

    /// <summary>
    /// Indicates that the XML is not well-formed or does not conform to the expected structure, resulting in a stream error.
    /// </summary>
    [XmppEnumMember("not-well-formed")]
    NotWellFormed,

    /// <summary>
    /// Indicates that the entity has violated a policy, such as sending too many stanzas in a short period of time.
    /// </summary>
    [XmppEnumMember("policy-violation")]
    PolicyViolation,

    /// <summary>
    /// Indicates that the connection to a remote server has failed.
    /// </summary>
    [XmppEnumMember("remote-connection-failed")]
    RemoteConnectionFailed,

    /// <summary>
    /// Indicates that the stream has been reset.
    /// </summary>
    [XmppEnumMember("reset")]
    Reset,

    /// <summary>
    /// Indicates that the server is experiencing resource constraints.
    /// </summary>
    [XmppEnumMember("resource-constraint")]
    ResourceConstraint,

    /// <summary>
    /// Indicates that the XML in the stanza is restricted.
    /// </summary>
    [XmppEnumMember("restricted-xml")]
    RestrictedXml,

    /// <summary>
    /// Indicates that the server is redirecting the client to another host.
    /// </summary>
    [XmppEnumMember("see-other-host")]
    SeeOtherHost,

    /// <summary>
    /// Indicates that the server is shutting down.
    /// </summary>
    [XmppEnumMember("system-shutdown")]
    SystemShutdown,

    /// <summary>
    /// Indicates that the stream error condition is undefined.
    /// </summary>
    [XmppEnumMember("undefined-condition")]
    UndefinedCondition,

    /// <summary>
    /// Indicates that the encoding used in the stream is not supported.
    /// </summary>
    [XmppEnumMember("unsupported-encoding")]
    UnsupportedEncoding,

    /// <summary>
    /// Indicates that the feature is not supported.
    /// </summary>
    [XmppEnumMember("unsupported-feature")]
    UnsupportedFeature,

    /// <summary>
    /// Indicates that the stanza type is not supported.
    /// </summary>
    [XmppEnumMember("unsupported-stanza-type")]
    UnsupportedStanzaType,

    /// <summary>
    /// Indicates that the XMPP protocol version is not supported.
    /// </summary>
    [XmppEnumMember("unsupported-version")]
    UnsupportedVersion
}
