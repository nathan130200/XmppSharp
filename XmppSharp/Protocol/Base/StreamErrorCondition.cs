using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppEnum]
public enum StreamErrorCondition
{
    Unspecified = -1,

    [XmppMember("bad-format")]
    BadFormat,

    [XmppMember("bad-namespace-prefix")]
    BadNamespacePrefix,

    [XmppMember("conflict")]
    Conflict,

    [XmppMember("connection-timeout")]
    ConnectionTimeout,

    [XmppMember("host-gone")]
    HostGone,

    [XmppMember("host-unknown")]
    HostUnknown,

    [XmppMember("improper-addressing")]
    ImproperAddressing,

    [XmppMember("internal-server-error")]
    InternalServerError,

    [XmppMember("invalid-from")]
    InvalidFrom,

    [XmppMember("invalid-namespace")]
    InvalidNamespace,

    [XmppMember("invalid-xml")]
    InvalidXml,

    [XmppMember("not-authorized")]
    NotAuthorized,

    [XmppMember("not-well-formed")]
    NotWellFormed,

    [XmppMember("policy-violation")]
    PolicyViolation,

    [XmppMember("remote-connection-failed")]
    RemoteConnectionFailed,

    [XmppMember("reset")]
    Reset,

    [XmppMember("resource-constraint")]
    ResourceConstraint,

    [XmppMember("restricted-xml")]
    RestrictedXml,

    [XmppMember("see-other-host")]
    SeeOtherHost,

    [XmppMember("system-shutdown")]
    SystemShutdown,

    [XmppMember("undefined-condition")]
    UndefinedCondition,

    [XmppMember("unsupported-encoding")]
    UnsupportedEncoding,

    [XmppMember("unsupported-feature")]
    UnsupportedFeature,

    [XmppMember("unsupported-stanza-type")]
    UnsupportedStanzaType,

    [XmppMember("unsupported-version")]
    UnsupportedVersion
}
