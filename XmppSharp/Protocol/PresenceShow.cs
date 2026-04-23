using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// Defines the presence show value, which indicates the availability status of a user in XMPP.
/// </summary>
public enum PresenceShow
{
    /// <summary>
    /// Indicates that the user is away from their device or not actively using it. This status is often used when a user is temporarily unavailable but still wants to receive messages or notifications.
    /// </summary>
    [XmppEnumMember("away")]
    Away = 1,

    /// <summary>
    /// Indicates that the user is available for chat. This status is often used when a user is actively engaged in conversation and wants to receive messages.
    /// </summary>
    [XmppEnumMember("chat")]
    Chat,

    /// <summary>
    /// Indicates that the user does not want to be disturbed. This status is often used when a user is busy or engaged in an activity and prefers not to receive messages.
    /// </summary>
    [XmppEnumMember("dnd")]
    DoNotDisturb,

    /// <summary>
    /// Indicates that the user is away for an extended period. This status is often used when a user may will not respond to messages promptly.
    /// </summary>
    [XmppEnumMember("xa")]
    ExtendedAway
}
