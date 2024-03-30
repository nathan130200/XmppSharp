using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Disco.IdentityValues;

/// <summary>
/// The server category consists of any Jabber/XMPP server.
/// </summary>
[XmppEnum]
public enum ServerValues
{
    /// <summary>
    /// Standard Jabber/XMPP server used for instant messaging and presence.
    /// </summary>
    [XmppMember("im")]
    IM
}