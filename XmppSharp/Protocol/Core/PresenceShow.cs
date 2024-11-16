using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Core;

[XmppEnum]
public enum PresenceShow
{
    [XmppMember("away")]
    Away,

    [XmppMember("chat")]
    Chat,

    [XmppMember("dnd")]
    DoNotDisturb,

    [XmppMember("xa")]
    ExtendedAway
}
