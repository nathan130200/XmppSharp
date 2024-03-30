using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The client category consists of different types of clients, mostly for instant messaging.
/// </summary>
[XmppEnum]
public enum ClientValues
{
    /// <summary>
    /// An automated client that is not controlled by a human user.
    /// </summary>
    [XmppMember("bot")]
    Bot,

    /// <summary>
    /// Minimal non-GUI client used on dumb terminals or text-only screens.
    /// </summary>
    [XmppMember("console")]
    Console,

    /// <summary>
    /// A client running on a gaming console.
    /// </summary>
    [XmppMember("game")]
    Game,

    /// <summary>
    /// A client running on a PDA, RIM device, or other handheld.
    /// </summary>
    [XmppMember("handheld")]
    Handheld,

    /// <summary>
    /// Standard full-GUI client used on desktops and laptops
    /// </summary>
    [XmppMember("pc")]
    PC,

    /// <summary>
    /// A client running on a mobile phone or other telephony device.
    /// </summary>
    [XmppMember("phone")]
    Phone,

    /// <summary>
    /// A client that is not actually using an instant messaging client; however, messages sent to this contact will be delivered as Short Message Service (SMS) messages.
    /// </summary>
    [XmppMember("sms")]
    Sms,

    /// <summary>
    /// A client running on a touchscreen device larger than a smartphone and without a physical keyboard permanently attached to it.
    /// </summary>
    [XmppMember("tablet")]
    Tablet,

    /// <summary>
    /// A client operated from within a web browser.
    /// </summary>
    [XmppMember("web")]
    Web,
}
