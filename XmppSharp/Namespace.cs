using System.Xml.Linq;

namespace XmppSharp;

public sealed class Namespace
{
    private readonly string _value;

    Namespace(string value)
        => _value = value;

    /// <summary>
    /// <a href="http://www.w3.org/XML/1998/namespace">W3C: Xml Namespace</a> — <c>http://www.w3.org/XML/1998/namespace</c>
    /// </summary>
    public static readonly Namespace Xml = new("http://www.w3.org/XML/1998/namespace");

    /// <summary>
    /// <a href="http://www.w3.org/2000/xmlns/">W3C: Xmlns Namespace</a> — <c>http://www.w3.org/2000/xmlns/</c>
    /// </summary>
    public static readonly Namespace Xmlns = new("http://www.w3.org/2000/xmlns/");

    /// <summary>
    /// <a href="http://www.w3.org/1999/xhtml">W3C: XHTML Namespace</a> — <c>http://www.w3.org/1999/xhtml</c>
    /// </summary>
    public static readonly Namespace XHtml = new("http://www.w3.org/1999/xhtml");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0108.html">XEP-0108: User Activity</a> — <c>http://jabber.org/protocol/activity</c>
    /// </summary>
    public static readonly Namespace UserActivity = new("http://jabber.org/protocol/activity");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0033.html">XEP-0033: Extended Stanza Addressing</a> — <c>http://jabber.org/protocol/address</c>
    /// </summary>
    public static readonly Namespace ExtendedStanzaAddressing = new("http://jabber.org/protocol/address");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0079.html">XEP-0079: Advanced Message Processing</a> — <c>http://jabber.org/protocol/amp</c>
    /// </summary>
    public static readonly Namespace Amp = new("http://jabber.org/protocol/amp");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0079.html">XEP-0079: Advanced Message Processing</a> — <c>http://jabber.org/protocol/amp#errors</c>
    /// </summary>
    public static readonly Namespace AmpErrors = new("http://jabber.org/protocol/amp#errors");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0065.html">XEP-0065: SOCKS5 Bytestreams</a> — <c>http://jabber.org/protocol/bytestreams</c>
    /// </summary>
    public static readonly Namespace ByteStreams = new("http://jabber.org/protocol/bytestreams");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0115.html">XEP-0115: Entity Capabilities</a> — <c>http://jabber.org/protocol/caps</c>
    /// </summary>
    public static readonly Namespace EntityCapabilities = new("http://jabber.org/protocol/caps");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0085.html">XEP-0085: Chat State Notifications</a> — <c>http://jabber.org/protocol/chatstates</c>
    /// </summary>
    public static readonly Namespace ChatStates = new("http://jabber.org/protocol/chatstates");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0050.html">XEP-0050: Ad-Hoc Commands</a> — <c>http://jabber.org/protocol/commands</c>
    /// </summary>
    public static readonly Namespace AdHoc = new("http://jabber.org/protocol/commands");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0138.html">XEP-0138: Stream Compression</a> — <c>http://jabber.org/protocol/compress</c>
    /// </summary>
    public static readonly Namespace StreamCompression = new("http://jabber.org/protocol/compress");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0030.html">XEP-0030: Service Discovery</a> — <c>http://jabber.org/protocol/disco#info</c>
    /// </summary>
    public static readonly Namespace DiscoInfo = new("http://jabber.org/protocol/disco#info");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0030.html">XEP-0030: Service Discovery</a> — <c>http://jabber.org/protocol/disco#items</c>
    /// </summary>
    public static readonly Namespace DiscoItems = new("http://jabber.org/protocol/disco#items");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0020.html">XEP-0020: Feature Negotiation</a> — <c>http://jabber.org/protocol/feature-neg</c>
    /// </summary>
    public static readonly Namespace FeatureNeg = new("http://jabber.org/protocol/feature-neg");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0080.html">XEP-0080: User Geolocation</a> — <c>http://jabber.org/protocol/geoloc</c>
    /// </summary>
    public static readonly Namespace GeoLoc = new("http://jabber.org/protocol/geoloc");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0070.html">XEP-0070: Verifying HTTP Requests via XMPP</a> — <c>http://jabber.org/protocol/http-auth</c>
    /// </summary>
    public static readonly Namespace HttpAuth = new("http://jabber.org/protocol/http-auth");

    /// <summary>
    /// <a href="XEP-0124: Bidirectional-streams Over Synchronous HTTP (BOSH)">XEP-0124: Bidirectional-streams Over Synchronous HTTP</a> — <c>http://jabber.org/protocol/httpbind</c>
    /// </summary>
    public static readonly Namespace HttpBind = new("http://jabber.org/protocol/httpbind");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0047.html">XEP-0047: In-Band Bytestreams</a> — <c>http://jabber.org/protocol/ibb</c>
    /// </summary>
    public static readonly Namespace Ibb = new("http://jabber.org/protocol/ibb");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0107.html">XEP-0107: User Mood</a> — <c>http://jabber.org/protocol/mood</c>
    /// </summary>
    public static readonly Namespace UserMood = new("http://jabber.org/protocol/mood");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0045.html">XEP-0045: Multi-User Chat</a> — <c>http://jabber.org/protocol/muc</c>
    /// </summary>
    public static readonly Namespace Muc = new("http://jabber.org/protocol/muc");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0045.html">XEP-0045: Multi-User Chat</a> — <c>http://jabber.org/protocol/muc#admin</c>
    /// </summary>
    public static readonly Namespace MucAdmin = new("http://jabber.org/protocol/muc#admin");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0045.html">XEP-0045: Multi-User Chat</a> — <c>http://jabber.org/protocol/muc#owner</c>
    /// </summary>
    public static readonly Namespace MucOwner = new("http://jabber.org/protocol/muc#owner");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0045.html">XEP-0045: Multi-User Chat</a> — <c>http://jabber.org/protocol/muc#user</c>
    /// </summary>
    public static readonly Namespace MucUser = new("http://jabber.org/protocol/muc#user");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0172.html">XEP-0172: User Nickname</a> — <c>http://jabber.org/protocol/nick</c>
    /// </summary>
    public static readonly Namespace Nick = new("http://jabber.org/protocol/nick");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0013.html">XEP-0013: Flexible Offline Message Retrieval</a> — <c>http://jabber.org/protocol/offline</c>
    /// </summary>
    public static readonly Namespace Offline = new("http://jabber.org/protocol/offline");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0112.html">XEP-0112: User Physical Location</a> — <c>http://jabber.org/protocol/physloc</c>
    /// </summary>
    public static readonly Namespace PhysicalLocation = new("http://jabber.org/protocol/physloc");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0060.html">XEP-0060: Publish-Subscribe</a> — <c>http://jabber.org/protocol/pubsub</c>
    /// </summary>
    public static readonly Namespace PubSub = new("http://jabber.org/protocol/pubsub");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0060.html">XEP-0060: Publish-Subscribe</a> — <c>http://jabber.org/protocol/pubsub#errors</c>
    /// </summary>
    public static readonly Namespace PubSubErrors = new("http://jabber.org/protocol/pubsub#errors");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0060.html">XEP-0060: Publish-Subscribe</a> — <c>http://jabber.org/protocol/pubsub#event</c>
    /// </summary>
    public static readonly Namespace PubSubEvent = new("http://jabber.org/protocol/pubsub#event");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0060.html">XEP-0060: Publish-Subscribe</a> — <c>http://jabber.org/protocol/pubsub#owner</c>
    /// </summary>
    public static readonly Namespace PubSubOwner = new("http://jabber.org/protocol/pubsub#owner");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0144.html">XEP-0144: Roster Item Exchange</a> — <c>http://jabber.org/protocol/rosterx</c>
    /// </summary>
    public static readonly Namespace RosterItemExchange = new("http://jabber.org/protocol/rosterx");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0141.html">XEP-0141: Data Forms Layout</a> — <c>http://jabber.org/protocol/xdata-layout</c>
    /// </summary>
    public static readonly Namespace DataFormsLayout = new("http://jabber.org/protocol/xdata-layout");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0122.html">XEP-0122: Data Forms Validation</a> — <c>http://jabber.org/protocol/xdata-validate</c>
    /// </summary>
    public static readonly Namespace DataFormsValidation = new("http://jabber.org/protocol/xdata-validate");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0114.html">XEP-0114: Existing Component Protocol</a> — <c>jabber:component:accept</c>
    /// </summary>
    public static readonly Namespace Accept = new("jabber:component:accept");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0114.html">XEP-0114: Existing Component Protocol</a> — <c>jabber:component:connect</c>
    /// </summary>
    public static readonly Namespace Connect = new("jabber:component:connect");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0078.html">XEP-0078: Non-SASL Authentication</a> — <c>jabber:iq:auth</c>
    /// </summary>
    public static readonly Namespace IqAuth = new("jabber:iq:auth");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0100.html">XEP-0100: Gateway Interaction</a> — <c>jabber:iq:gateway</c>
    /// </summary>
    public static readonly Namespace IqGateway = new("jabber:iq:gateway");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0012.html">XEP-0012: Last Activity</a> — <c>jabber:iq:last</c>
    /// </summary>
    public static readonly Namespace IqLast = new("jabber:iq:last");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0066.html">XEP-0066: Out of Band Data</a> — <c>jabber:iq:oob</c>
    /// </summary>
    public static readonly Namespace IqOob = new("jabber:iq:oob");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0016.html">XEP-0016: Privacy Lists</a> — <c>jabber:iq:privacy</c>
    /// </summary>
    public static readonly Namespace IqPrivacy = new("jabber:iq:privacy");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0049.html">XEP-0049: Private XML Storage</a> — <c>jabber:iq:private</c>
    /// </summary>
    public static readonly Namespace IqPrivate = new("jabber:iq:private");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0077.html">XEP-0077: In-Band Registration</a> — <c>jabber:iq:register</c>
    /// </summary>
    public static readonly Namespace IqRegister = new("jabber:iq:register");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0321.html">XEP-0321: Remote Roster Management</a> — <c>jabber:iq:roster</c>
    /// </summary>
    public static readonly Namespace IqRoster = new("jabber:iq:roster");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0009.html">XEP-0009: Jabber-RPC</a> — <c>jabber:iq:rpc</c>
    /// </summary>
    public static readonly Namespace IqRpc = new("jabber:iq:rpc");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0055.html">XEP-0055: Jabber Search</a> — <c>jabber:iq:search</c>
    /// </summary>
    public static readonly Namespace IqSearch = new("jabber:iq:search");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0092.html">XEP-0092: Software Version</a> — <c>jabber:iq:version</c>
    /// </summary>
    public static readonly Namespace IqVersion = new("jabber:iq:version");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0220.html">XEP-0220: Server Dialback</a> — <c>jabber:server:dialback</c>
    /// </summary>
    public static readonly Namespace Dialback = new("jabber:server:dialback");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0249.html">XEP-0249: Direct MUC Invitations</a> — <c>jabber:x:conference</c>
    /// </summary>
    public static readonly Namespace DirectMuc = new("jabber:x:conference");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0004.html">XEP-0004: Data Forms</a> — <c>jabber:x:data</c>
    /// </summary>
    public static readonly Namespace DataForms = new("jabber:x:data");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0066.html">XEP-0066: Out of Band Data</a> — <c>jabber:x:oob</c>
    /// </summary>
    public static readonly Namespace Oob = new("jabber:x:oob");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0083.html">XEP-0083: Nested Roster Groups</a> — <c>roster:delimiter</c>
    /// </summary>
    public static readonly Namespace RosterDdelimiter = new("roster:delimiter");

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>http://etherx.jabber.org/streams</c>
    /// </summary>
    public static readonly Namespace Stream = new("http://etherx.jabber.org/streams");

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>jabber:client</c>
    /// </summary>
    public static readonly Namespace Client = new("jabber:client");

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>jabber:server</c>
    /// </summary>
    public static readonly Namespace Server = new("jabber:server");

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-bind</c>
    /// </summary>
    public static readonly Namespace Bind = new("urn:ietf:params:xml:ns:xmpp-bind");

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-sasl</c>
    /// </summary>
    public static readonly Namespace Sasl = new("urn:ietf:params:xml:ns:xmpp-sasl");

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-session</c>
    /// </summary>
    public static readonly Namespace Session = new("urn:ietf:params:xml:ns:xmpp-session");

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-stanzas</c>
    /// </summary>
    public static readonly Namespace Stanzas = new("urn:ietf:params:xml:ns:xmpp-stanzas");

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-streams</c>
    /// </summary>
    public static readonly Namespace Streams = new("urn:ietf:params:xml:ns:xmpp-streams");

    /// <summary>
    /// <a href="https://xmpp.org/rfcs/rfc6120.html">RFC-6120: XMPP Core</a> — <c>urn:ietf:params:xml:ns:xmpp-tls</c>
    /// </summary>
    public static readonly Namespace Tls = new("urn:ietf:params:xml:ns:xmpp-tls");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0224.html">XEP-0224: Attention</a> — <c>urn:xmpp:attention:0</c>
    /// </summary>
    public static readonly Namespace Attention = new("urn:xmpp:attention:0");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0084.html">XEP-0084: User Avatars</a> — <c>urn:xmpp:avatar:data</c>
    /// </summary>
    public static readonly Namespace UserAvatarsData = new("urn:xmpp:avatar:data");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0084.html">XEP-0084: User Avatars</a> — <c>urn:xmpp:avatar:metadata</c>
    /// </summary>
    public static readonly Namespace UserAvatarsMetadata = new("urn:xmpp:avatar:metadata");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0288.html">XEP-0288: Bidirectional Server-to-Server Connections</a> — <c>urn:xmpp:bidi</c>
    /// </summary>
    public static readonly Namespace Bidi = new("urn:xmpp:bidi");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0231.html">XEP-0231: Bits of Binary</a> — <c>urn:xmpp:bob</c>
    /// </summary>
    public static readonly Namespace Bob = new("urn:xmpp:bob");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0158.html">XEP-0158: CAPTCHA Forms</a> — <c>urn:xmpp:captcha</c>
    /// </summary>
    public static readonly Namespace Captcha = new("urn:xmpp:captcha");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0203.html">XEP-0203: Delayed Delivery</a> — <c>urn:xmpp:delay</c>
    /// </summary>
    public static readonly Namespace Delay = new("urn:xmpp:delay");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0199.html">XEP-0199: XMPP Ping</a> — <c>urn:xmpp:ping</c>
    /// </summary>
    public static readonly Namespace Ping = new("urn:xmpp:ping");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0198.html">XEP-0198: Stream Management</a> — <c>urn:xmpp:sm:3</c>
    /// </summary>
    public static readonly Namespace StreamManagement = new("urn:xmpp:sm:3");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0202.html">XEP-0202: Entity Time</a> — <c>urn:xmpp:time</c>
    /// </summary>
    public static readonly Namespace EntityTime = new("urn:xmpp:time");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0054.html">XEP-0054: vcard-temp</a> — <c>vcard-temp</c>
    /// </summary>
    public static readonly Namespace vCard = new("vcard-temp");

    /// <summary>
    /// <a href="https://xmpp.org/extensions/xep-0153.html">XEP-0153: vCard-Based Avatars</a> — <c>vcard-temp:x:update</c>
    /// </summary>
    public static readonly Namespace vCardUpdate = new("vcard-temp:x:update");

    /// <summary>
    /// <c>urn:cryonline:k01</c>
    /// </summary>
    public static readonly Namespace CryOnline = new("urn:cryonline:k01");

    public override string ToString() => _value;

    public static implicit operator string(Namespace ns)
        => ns.ToString();

    public static implicit operator XNamespace(Namespace ns)
        => XNamespace.Get(ns._value);

    public static XName operator +(Namespace ns, string localName)
        => XName.Get(localName, ns);
}