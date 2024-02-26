using System.Xml.Linq;

namespace XmppSharp;

[RunStaticCtor]
public readonly struct Namespaces
{
	private readonly string _value;
	private readonly string? _prefix;

	// this is horrible.
	public Namespaces() => throw new InvalidOperationException("Don't use ctor.");

	Namespaces(string value, string? prefix = default)
	{
		_value = value;
		_prefix = prefix;
	}

	public static implicit operator XNamespace(Namespaces ns)
		=> ns._value;

	public XElement CreateElement(string tagName)
	{
		var result = new XElement(XName.Get(tagName, _value));

		if (!string.IsNullOrWhiteSpace(_prefix))
			result.Add(new XAttribute(XNamespace.Xmlns + _prefix, _value));

		return result;
	}

	public static implicit operator string(Namespaces ns)
		=> ns._value;

	/// <summary>
	/// XMLNS: <c>http://etherx.jabber.org/streams</c>
	/// </summary>
	public static Namespaces Stream { get; } = new("http://etherx.jabber.org/streams");

	/// <summary>
	/// XMLNS: <c>jabber:client</c>
	/// </summary>
	public static Namespaces Client { get; } = new("jabber:client");

	/// <summary>
	/// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-streams</c>
	/// </summary>
	public static Namespaces Streams { get; } = new("urn:ietf:params:xml:ns:xmpp-streams");

	/// <summary>
	/// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-tls</c>
	/// </summary>
	public static Namespaces Tls { get; } = new("urn:ietf:params:xml:ns:xmpp-tls");

	/// <summary>
	/// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-sasl</c>
	/// </summary>
	public static Namespaces Sasl { get; } = new("urn:ietf:params:xml:ns:xmpp-sasl");

	/// <summary>
	/// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-bind</c>
	/// </summary>
	public static Namespaces Bind { get; } = new("urn:ietf:params:xml:ns:xmpp-bind");

	/// <summary>
	/// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-session</c>
	/// </summary>
	public static Namespaces Session { get; } = new("urn:ietf:params:xml:ns:xmpp-session");

	/// <summary>
	/// XMLNS: <c>urn:ietf:params:xml:ns:xmpp-stanzas</c>
	/// </summary>
	public static Namespaces Stanzas { get; } = new("urn:ietf:params:xml:ns:xmpp-stanzas");

	/// <summary>
	/// XMLNS: <c>urn:cryonline:k01</c>
	/// </summary>
	public static Namespaces CryOnline { get; } = new("urn:cryonline:k01");
}
