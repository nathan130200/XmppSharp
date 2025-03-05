using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum PresenceType
{
    Available,

    [XmppMember("error")]
    Error,

    [XmppMember("probe")]
    Probe,

    [XmppMember("subscribe")]
    Subscribe,

    [XmppMember("subscribed")]
    Subscribed,

    [XmppMember("unavailable")]
    Unavailable,

    [XmppMember("unsubscribe")]
    Unsubscribe,

    [XmppMember("unsubscribed")]
    Unsubscribed,
}
