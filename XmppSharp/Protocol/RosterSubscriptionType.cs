using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum RosterSubscriptionType
{
    [XmppMember("none")]
    None,

    [XmppMember("remove")]
    Remove,

    [XmppMember("from")]
    From,

    [XmppMember("to")]
    To,

    [XmppMember("both")]
    Both,
}
