namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
    /// <summary>
    /// The "proxy" category consists of servers or services that act as special-purpose proxies or intermediaries between two or more XMPP endpoints.
    /// </summary>
    public static class Proxy
    {
        const string Name = "proxy";

        /// <summary>
        /// SOCKS5 bytestreams proxy service.
        /// </summary>
        public static Identity ByteStreams => new(Name, "bytestreams");
    }
}