using System.Xml;

namespace XmppSharp.Protocol;

[RunStaticCtor]
public readonly struct StreamErrorCondition : IXmppEnum<StreamErrorCondition>
{
    private readonly string _value;
    private readonly static Dictionary<string, StreamErrorCondition> s_cache = [];

    StreamErrorCondition(string value)
        => s_cache.Add(_value = value, this);

    #region IStructEnum Members

    public bool HasValue => _value != null;
    public string Value => _value;

    public static StreamErrorCondition Parse(string name)
    {
        if (s_cache.TryGetValue(name, out var value))
            return value;

        return default;
    }

    static object IXmppEnum.Parse(string value)
        => Parse(value);

    public static IEnumerable<StreamErrorCondition> Values
        => s_cache.Values;

    #endregion

    #region StreamErrorCondition Members

    public override int GetHashCode()
        => _value?.GetHashCode() ?? -1;

    public override bool Equals(object? obj)
    {
        if (obj is not StreamErrorCondition other)
            return false;

        if (!HasValue || !other.HasValue)
            return false;

        return _value.Equals(other._value);
    }

    public static bool operator ==(StreamErrorCondition lhs, StreamErrorCondition rhs)
        => lhs.Equals(rhs);

    public static bool operator !=(StreamErrorCondition lhs, StreamErrorCondition rhs)
        => !(lhs == rhs);

    public static implicit operator string(StreamErrorCondition self)
        => self._value;

    public override string ToString()
        => _value;

    public XmlElement CreateElement(string? message = default, string? language = "en", XmlDocument? document = default)
    {
        var element = Namespace.Stream.CreateElement("stream:error", document);

        element.C(_value, Namespace.Streams);

        if (!string.IsNullOrEmpty(message))
        {
            var text = element.C("text", Namespace.Streams);

            if (!string.IsNullOrWhiteSpace(language))
                text.SetAttribute("xml:lang", language);

            text.T(message);
        }

        return element;
    }

    #endregion

    /// <summary>
    /// The entity has sent XML that cannot be processed.
    /// </summary>
    public static StreamErrorCondition BadFormat { get; } = new("bad-format");

    /// <summary>
    /// The entity has sent a namespace prefix that is unsupported
    /// </summary>
    public static StreamErrorCondition BadNamespacePrefix { get; } = new("bad-namespace-prefix");

    /// <summary>
    /// The server either is closing the existing stream for this entity because a new stream has been initiated that conflicts with the existing stream or is refusing a new stream for this entity because allowing the new stream would conflict with an existing stream.
    /// </summary>
    public static StreamErrorCondition Conflict { get; } = new("conflict");

    /// <summary>
    /// One party is closing the stream because it has reason to believe that the other party has permanently lost the ability to communicate over the stream.
    /// </summary>
    public static StreamErrorCondition ConnectionTimeout { get; } = new("connection-timeout");

    /// <summary>
    /// The value of the <c>'to'</c> attribute provided in the initial stream header corresponds to an FQDN that is no longer serviced by the receiving entity.
    /// </summary>
    public static StreamErrorCondition HostGone { get; } = new("host-gone");

    /// <summary>
    /// The value of the <c>'to'</c> attribute provided in the initial stream header does not correspond to an FQDN that is serviced by the receiving entity.
    /// </summary>
    public static StreamErrorCondition HostUnknown { get; } = new("host-unknown");


    /// <summary>
    /// A stanza sent between two servers lacks a <c>'to'</c> or <c>'from'</c> attribute, the <c>'from'</c> or <c>'to'</c> attribute has no value, or the value violates the rules for XMPP addresses
    /// </summary>
    public static StreamErrorCondition ImproperAddressing { get; } = new("improper-addressing");

    /// <summary>
    /// The server has experienced a misconfiguration or other internal error that prevents it from servicing the stream.
    /// </summary>
    public static StreamErrorCondition InternalServerError { get; } = new("internal-server-error");

    /// <summary>
    /// The data provided in a <c>'from'</c> attribute does not match an authorized JID or validated domain as negotiated.
    /// </summary>
    public static StreamErrorCondition InvalidFrom { get; } = new("invalid-from");

    /// <summary>
    /// The stream namespace name is something other than <c>http://etherx.jabber.org/streams</c>.
    /// </summary>
    public static StreamErrorCondition InvalidNamespace { get; } = new("invalid-namespace");

    /// <summary>
    /// The entity has sent invalid XML over the stream to a server that performs validation.
    /// </summary>
    public static StreamErrorCondition InvalidXml { get; } = new("invalid-xml");

    /// <summary>
    /// The entity has attempted to send XML stanzas or other outbound data before the stream has been authenticated, or otherwise is not authorized to perform an action related to stream negotiation; the receiving entity MUST NOT process the offending data before sending the stream error.
    /// </summary>
    public static StreamErrorCondition NotAuthorized { get; } = new("not-authorized");

    /// <summary>
    ///  The initiating entity has sent XML that violates the well-formedness rules of <b>XML</b> or <b>XML NAMES</b>.
    /// </summary>
    public static StreamErrorCondition NotWellFormed { get; } = new("not-well-formed");

    /// <summary>
    /// The entity has violated some local service policy. 
    /// </summary>
    public static StreamErrorCondition PolicyViolation { get; } = new("policy-violation");

    /// <summary>
    /// The server is unable to properly connect to a remote entity that is needed for authentication or authorization.
    /// </summary>
    public static StreamErrorCondition RemoteConnectionFailed { get; } = new("remote-connection-failed");

    /// <summary>
    /// The server is closing the stream because it has new features to offer, because the keys or certificates used to establish a secure context for the stream have expired or have been revoked during the life of the stream, because the TLS sequence number has wrapped or other specific error.
    /// </summary>
    public static StreamErrorCondition Reset { get; } = new("reset");

    /// <summary>
    ///  The server lacks the system resources necessary to service the stream.
    /// </summary>
    public static StreamErrorCondition ResourceConstraint { get; } = new("resource-constraint");

    /// <summary>
    /// The entity has attempted to send restricted XML features such as a comment, processing instruction, DTD subset, or XML entity reference.
    /// </summary>
    public static StreamErrorCondition RestrictedXml { get; } = new("restricted-xml");

    /// <summary>
    /// The server will not provide service to the initiating entity but is redirecting traffic to another host under the administrative control of the same service provider.
    /// </summary>
    public static StreamErrorCondition SeeOtherHost { get; } = new("see-other-host");

    /// <summary>
    /// The server is being shut down and all active streams are being closed.
    /// </summary>
    public static StreamErrorCondition SystemShutdown { get; } = new("system-shutdown");

    /// <summary>
    /// The error condition is not one of those defined by the other conditions in this list; this error condition SHOULD NOT be used except in conjunction with an application-specific condition.
    /// </summary>
    public static StreamErrorCondition UndefinedCondition { get; } = new("undefined-condition");

    /// <summary>
    /// The initiating entity has encoded the stream in an encoding that is not supported by the server or has otherwise improperly encoded the stream.
    /// </summary>
    public static StreamErrorCondition UnsupportedEncoding { get; } = new("unsupported-encoding");

    /// <summary>
    /// The receiving entity has advertised a mandatory-to-negotiate stream feature that the initiating entity does not support, and has offered no other mandatory-to-negotiate feature alongside the unsupported feature.
    /// </summary>
    public static StreamErrorCondition UnsupportedFeature { get; } = new("unsupported-feature");

    /// <summary>
    /// The initiating entity has sent a first-level child of the stream that is not supported by the server, either because the receiving entity does not understand the namespace or because the receiving entity does not understand the element name for the applicable namespace (which might be the content namespace declared as the default namespace).
    /// </summary>
    public static StreamErrorCondition UnsupportedStanzaType { get; } = new("unsupported-stanza-type");

    /// <summary>
    /// The <c>'version'</c> attribute provided by the initiating entity in the stream header specifies a version of XMPP that is not supported by the server.
    /// </summary>
    public static StreamErrorCondition UnsupportedVersion { get; } = new("unsupported-version");
}
