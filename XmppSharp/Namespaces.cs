namespace XmppSharp;

/// <summary>
/// Defines common XML namespaces used in XMPP (Extensible Messaging and Presence Protocol).
/// </summary>
public static class Namespaces
{
    /// <summary>
    /// The XML namespace for standard XML attributes and elements, such as xml:lang and xml:space.
    /// </summary>
    public const string Xml = "http://www.w3.org/XML/1998/namespace";

    /// <summary>
    /// The XML namespace for XML namespace declarations, used to define and manage namespaces in XML documents.
    /// </summary>
    public const string Xmlns = "http://www.w3.org/2000/xmlns/";

    /// <summary>
    /// The XML namespace for XMPP streams, which is used to define the structure and semantics of XMPP stream elements.
    /// </summary>
    public const string Stream = "http://etherx.jabber.org/streams";

    /// <summary>
    /// The XML namespace for XMPP clients, which is used to define the structure and semantics of XMPP stanzas and elements.
    /// </summary>
    public const string Client = "jabber:client";

    /// <summary>
    /// The XML namespace for XMPP components, which is used to define the structure and semantics of XMPP stanzas and elements.
    /// </summary>
    public const string Component = "jabber:component:accept";

    /// <summary>
    /// The XML namespace for XMPP servers, which is used to define the structure and semantics of XMPP stanzas and elements.
    /// </summary>
    public const string Server = "jabber:server";

    /// <summary>
    /// The XML namespace for XMPP TLS (Transport Layer Security).
    /// </summary>
    public const string Tls = "urn:ietf:params:xml:ns:xmpp-tls";

    /// <summary>
    /// The XML namespace for XMPP SASL (Simple Authentication and Security Layer)..
    /// </summary>
    public const string Sasl = "urn:ietf:params:xml:ns:xmpp-sasl";

    /// <summary>
    /// The XML namespace for XMPP session management.
    /// </summary>
    public const string Session = "urn:ietf:params:xml:ns:xmpp-session";

    /// <summary>
    /// The XML namespace for XMPP resource binding.
    /// </summary>
    public const string Bind = "urn:ietf:params:xml:ns:xmpp-bind";

    /// <summary>
    /// The XML namespace for XMPP streams.
    /// </summary>
    public const string Streams = "urn:ietf:params:xml:ns:xmpp-streams";

    /// <summary>
    /// The XML namespace for XMPP stanzas.
    /// </summary>
    public const string Stanzas = "urn:ietf:params:xml:ns:xmpp-stanzas";

    /// <summary>
    /// The XML namespace for XMPP multi-user chat (MUC).
    /// </summary>
    public const string Muc = "http://jabber.org/protocol/muc";

    /// <summary>
    /// The XML namespaces for XMPP multi-user chat (MUC) roles and affiliations.
    /// </summary>
    public const string MucAdmin = "http://jabber.org/protocol/muc#admin";

    /// <summary>
    /// The XML namespaces for XMPP multi-user chat (MUC) user information and presence.
    /// </summary>
    public const string MucUser = "http://jabber.org/protocol/muc#user";

    /// <summary>
    /// The XML namespaces for XMPP multi-user chat (MUC) room ownership and configuration.
    /// </summary>
    public const string MucOwner = "http://jabber.org/protocol/muc#owner";

    /// <summary>
    /// The XML namespace for XMPP service discovery (disco), which is used to discover information about XMPP entities and their capabilities.
    /// </summary>
    public const string DiscoInfo = "http://jabber.org/protocol/disco#info";

    /// <summary>
    /// The XML namespace for XMPP service discovery (disco) items, which is used to discover items associated with XMPP entities, such as rooms, services, or features.
    /// </summary>
    public const string DiscoItems = "http://jabber.org/protocol/disco#items";

    /// <summary>
    /// The XML namespace which is an internal namespace used by the Warface game for XMPP communication and features specific to the game's online services.
    /// </summary>
    public const string CryOnline = "urn:cryonline:k01";

    /// <summary>
    /// The XML namespace for XMPP ping, which is used to check the availability and responsiveness of XMPP entities.
    /// </summary>
    public const string Ping = "urn:xmpp:ping";
}