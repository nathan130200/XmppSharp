using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// Defines the type of presence information being sent or received in XMPP communication.
/// </summary>
public enum PresenceType
{
    /// <summary>
    /// Indicates that the entity is available and can receive messages or participate in the chat. This is the default presence type when a user is online and active.
    /// </summary>
    Available,

    /// <summary>
    /// Indicates that there is an error related to the presence information.
    /// </summary>
    [XmppEnumMember("error")]
    Error,

    /// <summary>
    /// Indicates that the entity is being probed for its presence information. This is typically used by a server or another entity to check if a user is online and available.
    /// </summary>
    [XmppEnumMember("probe")]
    Probe,

    /// <summary>
    /// Indicates that the entity is requesting to subscribe to another entity's presence information.
    /// </summary>
    [XmppEnumMember("subscribe")]
    Subscribe,

    /// <summary>
    /// Indicates that the entity has accepted a subscription request and is now subscribed to another entity's presence information.
    /// </summary>
    [XmppEnumMember("subscribed")]
    Subscribed,

    /// <summary>
    /// Indicates that the entity is unavailable and cannot receive messages or participate in the chat.
    /// </summary>
    [XmppEnumMember("unavailable")]
    Unavailable,

    /// <summary>
    /// Indicates that the entity is requesting to unsubscribe from another entity's presence information.
    /// </summary>
    [XmppEnumMember("unsubscribe")]
    Unsubscribe,

    /// <summary>
    /// Indicates that the entity has acknowledged an unsubscribe request and is no longer subscribed to another entity's presence information.
    /// </summary>
    [XmppEnumMember("unsubscribed")]
    Unsubscribed,
}
