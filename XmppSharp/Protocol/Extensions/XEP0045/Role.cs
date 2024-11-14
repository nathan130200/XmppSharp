using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[XmppEnum]
public enum Role
{
    [XmppMember("none")]
    None,

    [XmppMember("moderator")]
    Moderator,

    [XmppMember("participant")]
    Participant,

    [XmppMember("visitor")]
    Visitor,
}
