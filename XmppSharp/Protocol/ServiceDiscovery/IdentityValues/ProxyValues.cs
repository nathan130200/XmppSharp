using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The proxy category consists of servers or services that act as special-purpose proxies or intermediaries between two or more XMPP endpoints.
/// </summary>
[XmppEnum]
public enum ProxyValues
{
	/// <summary>
	/// SOCKS5 bytestreams proxy service.
	/// </summary>
	[XmppMember("bytestreams")]
	ByteStreams
}