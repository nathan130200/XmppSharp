using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The component category consists of services that are internal to server implementations and not normally exposed outside a server.
/// </summary>
[XmppEnum]
public enum ComponentValues
{
    /// <summary>
    /// A server component that archives traffic.
    /// </summary>
    [XmppMember("archive")]
    Archive,

    /// <summary>
    /// A server component that handles client connections.
    /// </summary>
    [XmppMember("c2s")]
    C2S,

    /// <summary>
    /// A server component other than one of the registered types.
    /// </summary>
    [XmppMember("generic")]
    Generic,

    /// <summary>
    /// A server component that handles load balancing.
    /// </summary>
    [XmppMember("load")]
    Load,

    /// <summary>
    /// A server component that logs server information.
    /// </summary>
    [XmppMember("log")]
    Log,

    /// <summary>
    /// A server component that provides presence information.
    /// </summary>
    [XmppMember("presence")]
    Presence,

    /// <summary>
    /// A server component that handles core routing logic.
    /// </summary>
    [XmppMember("router")]
    Router,

    /// <summary>
    /// A server component that handles server connections.
    /// </summary>
    [XmppMember("s2s")]
    S2S,

    /// <summary>
    /// A server component that manages user sessions.
    /// </summary>
    [XmppMember("sm")]
    SM,

    /// <summary>
    /// A server component that provides server statistics.
    /// </summary>
    [XmppMember("stats")]
    Stats
}
