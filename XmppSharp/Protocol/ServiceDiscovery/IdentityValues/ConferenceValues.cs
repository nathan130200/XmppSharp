using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The conference category consists of online conference services such as multi-user chatroom services.
/// </summary>
[XmppEnum]
public enum ConferenceValues
{
    /// <summary>
    /// Internet Relay Chat service.
    /// </summary>
    [XmppMember("irc")]
    Irc,

    /// <summary>
    /// Text conferencing service.
    /// </summary>
    [XmppMember("text")]
    Text,
}
