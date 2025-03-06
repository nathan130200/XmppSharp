namespace XmppSharp;

/// <summary>
/// Collection of common namespaces used in the XMPP protocol.
/// </summary>
public static class Namespaces
{
    /// <summary>
    /// <a href="http://www.w3.org/XML/1998/namespace">W3C: Xml Namespace</a> — <c>http://www.w3.org/XML/1998/namespace</c>
    /// </summary>
    public const string Xml = "http://www.w3.org/XML/1998/namespace";

    /// <summary>
    /// <a href="http://www.w3.org/2000/xmlns/">W3C: Xmlns Namespace</a> — <c>http://www.w3.org/2000/xmlns/</c>
    /// </summary>
    public const string Xmlns = "http://www.w3.org/2000/xmlns/";

    /// <summary>
    /// <a href="http://www.w3.org/1999/xhtml">W3C: XHTML Namespace</a> — <c>http://www.w3.org/1999/xhtml</c>
    /// </summary>
    public const string XHtml = "http://www.w3.org/1999/xhtml";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0108.html">XEP-0108: User Activity</a> — <c>http://jabber.org/protocol/activity</c>
    /// </summary>
    public const string UserActivity = "http://jabber.org/protocol/activity";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0033.html">XEP-0033: Extended Stanza Addressing</a> — <c>http://jabber.org/protocol/address</c>
    /// </summary>
    public const string ExtendedStanzaAddressing = "http://jabber.org/protocol/address";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0079.html">XEP-0079: Advanced Message Processing</a> — <c>http://jabber.org/protocol/amp</c>
    /// </summary>
    public const string Amp = "http://jabber.org/protocol/amp";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0079.html">XEP-0079: Advanced Message Processing</a> — <c>http://jabber.org/protocol/amp#errors</c>
    /// </summary>
    public const string AmpErrors = "http://jabber.org/protocol/amp#errors";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0065.html">XEP-0065: SOCKS5 Bytestreams</a> — <c>http://jabber.org/protocol/bytestreams</c>
    /// </summary>
    public const string SocksByteStreams = "http://jabber.org/protocol/bytestreams";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0115.html">XEP-0115: Entity Capabilities</a> — <c>http://jabber.org/protocol/caps</c>
    /// </summary>
    public const string EntityCapabilities = "http://jabber.org/protocol/caps";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0085.html">XEP-0085: Chat State Notifications</a> — <c>http://jabber.org/protocol/chatstates</c>
    /// </summary>
    public const string ChatStates = "http://jabber.org/protocol/chatstates";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0050.html">XEP-0050: Ad-Hoc Commands</a> — <c>http://jabber.org/protocol/commands</c>
    /// </summary>
    public const string AdHoc = "http://jabber.org/protocol/commands";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0138.html">XEP-0138: Stream Compression</a> — <c>http://jabber.org/protocol/compress</c>
    /// </summary>
    public const string StreamCompression = "http://jabber.org/protocol/compress";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0030.html">XEP-0030: Service Discovery</a> — <c>http://jabber.org/protocol/disco#info</c>
    /// </summary>
    public const string DiscoInfo = "http://jabber.org/protocol/disco#info";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0030.html">XEP-0030: Service Discovery</a> — <c>http://jabber.org/protocol/disco#items</c>
    /// </summary>
    public const string DiscoItems = "http://jabber.org/protocol/disco#items";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0020.html">XEP-0020: Feature Negotiation</a> — <c>http://jabber.org/protocol/feature-neg</c>
    /// </summary>
    public const string FeatureNeg = "http://jabber.org/protocol/feature-neg";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0080.html">XEP-0080: User Geolocation</a> — <c>http://jabber.org/protocol/geoloc</c>
    /// </summary>
    public const string GeoLoc = "http://jabber.org/protocol/geoloc";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0070.html">XEP-0070: Verifying HTTP Requests via XMPP</a> — <c>http://jabber.org/protocol/http-auth</c>
    /// </summary>
    public const string HttpAuth = "http://jabber.org/protocol/http-auth";

    /// <summary>
    /// <a href="XEP-0124: Bidirectional-streams Over Synchronous HTTP (BOSH)">XEP-0124: Bidirectional-streams Over Synchronous HTTP</a> — <c>http://jabber.org/protocol/httpbind</c>
    /// </summary>
    public const string HttpBind = "http://jabber.org/protocol/httpbind";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0047.html">XEP-0047: In-Band Bytestreams</a> — <c>http://jabber.org/protocol/ibb</c>
    /// </summary>
    public const string Ibb = "http://jabber.org/protocol/ibb";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0107.html">XEP-0107: User Mood</a> — <c>http://jabber.org/protocol/mood</c>
    /// </summary>
    public const string UserMood = "http://jabber.org/protocol/mood";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0045.html">XEP-0045: Multi-User Chat</a> — <c>http://jabber.org/protocol/muc</c>
    /// </summary>
    public const string Muc = "http://jabber.org/protocol/muc";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0045.html">XEP-0045: Multi-User Chat</a> — <c>http://jabber.org/protocol/muc#admin</c>
    /// </summary>
    public const string MucAdmin = "http://jabber.org/protocol/muc#admin";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0045.html">XEP-0045: Multi-User Chat</a> — <c>http://jabber.org/protocol/muc#owner</c>
    /// </summary>
    public const string MucOwner = "http://jabber.org/protocol/muc#owner";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0045.html">XEP-0045: Multi-User Chat</a> — <c>http://jabber.org/protocol/muc#user</c>
    /// </summary>
    public const string MucUser = "http://jabber.org/protocol/muc#user";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0172.html">XEP-0172: User Nickname</a> — <c>http://jabber.org/protocol/nick</c>
    /// </summary>
    public const string Nick = "http://jabber.org/protocol/nick";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0013.html">XEP-0013: Flexible Offline Message Retrieval</a> — <c>http://jabber.org/protocol/offline</c>
    /// </summary>
    public const string Offline = "http://jabber.org/protocol/offline";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0112.html">XEP-0112: User Physical Location</a> — <c>http://jabber.org/protocol/physloc</c>
    /// </summary>
    public const string PhysicalLocation = "http://jabber.org/protocol/physloc";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0060.html">XEP-0060: Publish-Subscribe</a> — <c>http://jabber.org/protocol/pubsub</c>
    /// </summary>
    public const string PubSub = "http://jabber.org/protocol/pubsub";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0060.html">XEP-0060: Publish-Subscribe</a> — <c>http://jabber.org/protocol/pubsub#errors</c>
    /// </summary>
    public const string PubSubErrors = "http://jabber.org/protocol/pubsub#errors";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0060.html">XEP-0060: Publish-Subscribe</a> — <c>http://jabber.org/protocol/pubsub#event</c>
    /// </summary>
    public const string PubSubEvent = "http://jabber.org/protocol/pubsub#event";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0060.html">XEP-0060: Publish-Subscribe</a> — <c>http://jabber.org/protocol/pubsub#owner</c>
    /// </summary>
    public const string PubSubOwner = "http://jabber.org/protocol/pubsub#owner";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0144.html">XEP-0144: Roster Item Exchange</a> — <c>http://jabber.org/protocol/rosterx</c>
    /// </summary>
    public const string RosterItemExchange = "http://jabber.org/protocol/rosterx";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0141.html">XEP-0141: Data Forms Layout</a> — <c>http://jabber.org/protocol/xdata-layout</c>
    /// </summary>
    public const string DataFormsLayout = "http://jabber.org/protocol/xdata-layout";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0122.html">XEP-0122: Data Forms Validation</a> — <c>http://jabber.org/protocol/xdata-validate</c>
    /// </summary>
    public const string DataFormsValidation = "http://jabber.org/protocol/xdata-validate";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0114.html">XEP-0114: Existing Component Protocol</a> — <c>jabber:component:accept</c>
    /// </summary>
    public const string Accept = "jabber:component:accept";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0114.html">XEP-0114: Existing Component Protocol</a> — <c>jabber:component:connect</c>
    /// </summary>
    public const string Connect = "jabber:component:connect";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0078.html">XEP-0078: Non-SASL Authentication</a> — <c>jabber:iq:auth</c>
    /// </summary>
    public const string IqAuth = "jabber:iq:auth";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0100.html">XEP-0100: Gateway Interaction</a> — <c>jabber:iq:gateway</c>
    /// </summary>
    public const string IqGateway = "jabber:iq:gateway";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0012.html">XEP-0012: Last Activity</a> — <c>jabber:iq:last</c>
    /// </summary>
    public const string IqLast = "jabber:iq:last";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0066.html">XEP-0066: Out of Band Data</a> — <c>jabber:iq:oob</c>
    /// </summary>
    public const string IqOob = "jabber:iq:oob";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0016.html">XEP-0016: Privacy Lists</a> — <c>jabber:iq:privacy</c>
    /// </summary>
    public const string IqPrivacy = "jabber:iq:privacy";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0049.html">XEP-0049: Private XML Storage</a> — <c>jabber:iq:private</c>
    /// </summary>
    public const string IqPrivate = "jabber:iq:private";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0077.html">XEP-0077: In-Band Registration</a> — <c>jabber:iq:register</c>
    /// </summary>
    public const string IqRegister = "jabber:iq:register";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0321.html">XEP-0321: Remote Roster Management</a> — <c>jabber:iq:roster</c>
    /// </summary>
    public const string IqRoster = "jabber:iq:roster";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0009.html">XEP-0009: Jabber-RPC</a> — <c>jabber:iq:rpc</c>
    /// </summary>
    public const string IqRpc = "jabber:iq:rpc";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0055.html">XEP-0055: Jabber Search</a> — <c>jabber:iq:search</c>
    /// </summary>
    public const string IqSearch = "jabber:iq:search";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0092.html">XEP-0092: Software Version</a> — <c>jabber:iq:version</c>
    /// </summary>
    public const string IqVersion = "jabber:iq:version";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0220.html">XEP-0220: Server Dialback</a> — <c>jabber:server:dialback</c>
    /// </summary>
    public const string Dialback = "jabber:server:dialback";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0249.html">XEP-0249: Direct MUC Invitations</a> — <c>jabber:x:conference</c>
    /// </summary>
    public const string DirectMuc = "jabber:x:conference";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0004.html">XEP-0004: Data Forms</a> — <c>jabber:x:data</c>
    /// </summary>
    public const string DataForms = "jabber:x:data";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0066.html">XEP-0066: Out of Band Data</a> — <c>jabber:x:oob</c>
    /// </summary>
    public const string Oob = "jabber:x:oob";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0083.html">XEP-0083: Nested Roster Groups</a> — <c>roster:delimiter</c>
    /// </summary>
    public const string RosterDdelimiter = "roster:delimiter";

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>http://etherx.jabber.org/streams</c>
    /// </summary>
    public const string Stream = "http://etherx.jabber.org/streams";

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>jabber:client</c>
    /// </summary>
    public const string Client = "jabber:client";

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>jabber:server</c>
    /// </summary>
    public const string Server = "jabber:server";

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-bind</c>
    /// </summary>
    public const string Bind = "urn:ietf:params:xml:ns:xmpp-bind";

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-sasl</c>
    /// </summary>
    public const string Sasl = "urn:ietf:params:xml:ns:xmpp-sasl";

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-session</c>
    /// </summary>
    public const string Session = "urn:ietf:params:xml:ns:xmpp-session";

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-stanzas</c>
    /// </summary>
    public const string Stanzas = "urn:ietf:params:xml:ns:xmpp-stanzas";

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-streams</c>
    /// </summary>
    public const string Streams = "urn:ietf:params:xml:ns:xmpp-streams";

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-tls</c>
    /// </summary>
    public const string Tls = "urn:ietf:params:xml:ns:xmpp-tls";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0224.html">XEP-0224: Attention</a> — <c>urn:xmpp:attention:0</c>
    /// </summary>
    public const string Attention = "urn:xmpp:attention:0";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0084.html">XEP-0084: User Avatars</a> — <c>urn:xmpp:avatar:data</c>
    /// </summary>
    public const string UserAvatarsData = "urn:xmpp:avatar:data";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0084.html">XEP-0084: User Avatars</a> — <c>urn:xmpp:avatar:metadata</c>
    /// </summary>
    public const string UserAvatarsMetadata = "urn:xmpp:avatar:metadata";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0288.html">XEP-0288: Bidirectional Server-to-Server Connections</a> — <c>urn:xmpp:bidi</c>
    /// </summary>
    public const string Bidi = "urn:xmpp:bidi";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0231.html">XEP-0231: Bits of Binary</a> — <c>urn:xmpp:bob</c>
    /// </summary>
    public const string Bob = "urn:xmpp:bob";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0158.html">XEP-0158: CAPTCHA Forms</a> — <c>urn:xmpp:captcha</c>
    /// </summary>
    public const string Captcha = "urn:xmpp:captcha";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0203.html">XEP-0203: Delayed Delivery</a> — <c>urn:xmpp:delay</c>
    /// </summary>
    public const string Delay = "urn:xmpp:delay";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0199.html">XEP-0199: XMPP Ping</a> — <c>urn:xmpp:ping</c>
    /// </summary>
    public const string Ping = "urn:xmpp:ping";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0198.html">XEP-0198: Stream Management</a> — <c>urn:xmpp:sm:3</c>
    /// </summary>
    public const string StreamManagement = "urn:xmpp:sm:3";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0202.html">XEP-0202: Entity Time</a> — <c>urn:xmpp:time</c>
    /// </summary>
    public const string EntityTime = "urn:xmpp:time";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0054.html">XEP-0054: vcard-temp</a> — <c>vcard-temp</c>
    /// </summary>
    public const string vCard = "vcard-temp";

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0153.html">XEP-0153: vCard-Based Avatars</a> — <c>vcard-temp:x:update</c>
    /// </summary>
    public const string vCardUpdate = "vcard-temp:x:update";

    /// <summary>
    /// <a href="">Cry Online</a> - <c>urn:cryonline:k01</c>
    /// </summary>
    public const string CryOnline = "urn:cryonline:k01";
}