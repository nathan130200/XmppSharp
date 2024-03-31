using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum PresenceType
{
    [XmppMember("error")]
    Error,

    Available,

    [XmppMember("unavailable")]
    Unavailable,

    [XmppMember("probe")]
    Probe,

    [XmppMember("subscribe")]
    Subscribe,

    [XmppMember("subscribed")]
    Subscribed,

    [XmppMember("unsubscribe")]
    Unsubscribe,

    [XmppMember("unsubscribed")]
    Unsubscribed
}