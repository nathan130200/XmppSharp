using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.MultiUserChat;

[XmppEnum]
public enum Affiliation
{
    [XmppMember("none")]
    None,

    [XmppMember("outcast")]
    Outcast,

    [XmppMember("member")]
    Member,

    [XmppMember("admin")]
    Admin,

    [XmppMember("owner")]
    Owner,
}
