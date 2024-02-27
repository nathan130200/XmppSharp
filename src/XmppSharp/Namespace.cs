using XmppSharp.Attributes;

namespace XmppSharp;

[BetterEnum]
public enum Namespace
{
	/// <summary>
	/// <c>http://etherx.jabber.org/streams</c>
	/// </summary>
	[XmppNamespace("http://etherx.jabber.org/streams", Prefix = "stream")]
	Stream,

	/// <summary>
	/// <c>jabber:client</c>
	/// </summary>
	[XmppNamespace("jabber:client")]
	Client,

	/// <summary>
	/// <c>urn:ietf:params:xml:ns:xmpp-streams</c>
	/// </summary>
	[XmppNamespace("urn:ietf:params:xml:ns:xmpp-streams")]
	Streams,

	/// <summary>
	/// <c>urn:ietf:params:xml:ns:xmpp-tls</c>
	/// </summary>
	[XmppNamespace("urn:ietf:params:xml:ns:xmpp-tls")]
	Tls,

	/// <summary>
	/// <c>urn:ietf:params:xml:ns:xmpp-sasl</c>
	/// </summary>
	[XmppNamespace("urn:ietf:params:xml:ns:xmpp-sasl")]
	Sasl,

	/// <summary>
	/// <c>urn:ietf:params:xml:ns:xmpp-bind</c>
	/// </summary>
	[XmppNamespace("urn:ietf:params:xml:ns:xmpp-bind")]
	Bind,

	/// <summary>
	/// <c>urn:ietf:params:xml:ns:xmpp-session</c>
	/// </summary>
	[XmppNamespace("urn:ietf:params:xml:ns:xmpp-session")]
	Session,

	/// <summary>
	/// <c>urn:ietf:params:xml:ns:xmpp-stanzas</c>
	/// </summary>
	[XmppNamespace("urn:ietf:params:xml:ns:xmpp-stanzas")]
	Stanzas,

	/// <summary>
	/// <c>urn:cryonline:k01</c>
	/// </summary>
	[XmppNamespace("urn:cryonline:k01")]
	CryOnline
}
