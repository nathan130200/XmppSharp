using System.Xml;

namespace Jabber;

[RunStaticCtor]
public readonly struct Namespace
{
    private readonly string _value;

    Namespace(string value)
        => _value = value;

    bool HasValue => _value != null;

    #region Namespace Members

    public override int GetHashCode()
        => _value?.GetHashCode() ?? -1;

    public override bool Equals(object? obj)
    {
        if (obj is not Namespace other)
            return false;

        if (!HasValue || !other.HasValue)
            return false;

        return _value.Equals(other._value);
    }

    public static bool operator ==(Namespace lhs, Namespace rhs)
        => lhs.Equals(rhs);

    public static bool operator !=(Namespace lhs, Namespace rhs)
        => !(lhs == rhs);

    public XmlElement CreateElement(string name, XmlDocument? document = default)
        => Xml.Element(name, _value, document);

    #endregion

    /// <summary>
    /// XMLNS: <c>http://etherx.jabber.org/streams</c>
    /// </summary>
    public static Namespace Stream { get; } = new("http://etherx.jabber.org/streams");

    /// <summary>
    /// XMLNS: <c>jabber:client</c>
    /// </summary>
    public static Namespace Client { get; } = new("jabber:client");

    /// <summary>
    /// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-streams</c>
    /// </summary>
    public static Namespace Streams { get; } = new("urn:ietf:params:xml:ns:xmpp-streams");

    /// <summary>
    /// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-tls</c>
    /// </summary>
    public static Namespace Tls { get; } = new("urn:ietf:params:xml:ns:xmpp-tls");

    /// <summary>
    /// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-sasl</c>
    /// </summary>
    public static Namespace Sasl { get; } = new("urn:ietf:params:xml:ns:xmpp-sasl");

    /// <summary>
    /// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-bind</c>
    /// </summary>
    public static Namespace Bind { get; } = new("urn:ietf:params:xml:ns:xmpp-bind");

    /// <summary>
    /// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-session</c>
    /// </summary>
    public static Namespace Session { get; } = new("urn:ietf:params:xml:ns:xmpp-session");

    /// <summary>
    /// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-stanzas</c>
    /// </summary>
    public static Namespace Stanzas { get; } = new("urn:ietf:params:xml:ns:xmpp-stanzas");

    /// <summary>
    /// XMLNS: <c>urn:cryonline:k01</c>
    /// </summary>
    public static Namespace CryOnline { get; } = new("urn:cryonline:k01");

    public static implicit operator string(Namespace v)
        => v._value;
}
