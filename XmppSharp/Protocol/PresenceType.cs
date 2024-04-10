using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum PresenceType
{
    /// <summary>
    /// An error has occurred regarding processing or delivery of a previously-sent presence stanza.
    /// </summary>
    [XmppMember("error")]
    Error,

    /// <summary>
    /// Signal to the server that the sender is online and available for communication.
    /// </summary>
    Available,

    /// <summary>
    /// Signals that the entity is no longer available for communication.
    /// </summary>
    [XmppMember("unavailable")]
    Unavailable,

    /// <summary>
    /// A request for an entity's current presence. <b>Should</b> be generated only by a server on behalf of a user.
    /// </summary>
    [XmppMember("probe")]
    Probe,

    /// <summary>
    /// The sender wishes to subscribe to the recipient's presence.
    /// </summary>
    [XmppMember("subscribe")]
    Subscribe,

    /// <summary>
    /// The sender has allowed the recipient to receive their presence.
    /// </summary>
    [XmppMember("subscribed")]
    Subscribed,

    /// <summary>
    /// The sender is unsubscribing from another entity's presence.
    /// </summary>
    [XmppMember("unsubscribe")]
    Unsubscribe,

    /// <summary>
    /// The subscription request has been denied or a previously-granted subscription has been cancelled.
    /// </summary>
    [XmppMember("unsubscribed")]
    Unsubscribed
}