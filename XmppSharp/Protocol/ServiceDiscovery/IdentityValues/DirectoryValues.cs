using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The directory category consists of information retrieval services that enable users to search online directories or otherwise be informed about the existence of other XMPP entities.
/// </summary>
[XmppEnum]
public enum DirectoryValues
{
    /// <summary>
    /// A directory of chatrooms.
    /// </summary>
    [XmppMember("chatroom")]
    Chatroom,

    /// <summary>
    /// A directory that provides shared roster groups.
    /// </summary>
    [XmppMember("group")]
    Group,

    /// <summary>
    /// A directory of end users.
    /// </summary>
    [XmppMember("user")]
    User,

    /// <summary>
    /// A directory of waiting list entries.
    /// </summary>
    [XmppMember("waitinglist")]
    WaitingList
}
