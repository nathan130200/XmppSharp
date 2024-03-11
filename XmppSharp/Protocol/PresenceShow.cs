using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum PresenceShow
{
    [XmppEnumMember("away")]
    Away,

    [XmppEnumMember("chat")]
    Chat,

    [XmppEnumMember("dnd")]
    DoNotDisturb,

    [XmppEnumMember("xa")]
    ExtendedAway
}