using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// Services and nodes that adhere to <b>XEP-0060</b>.
/// </summary>
[XmppEnum]
public enum PubSubValues
{
    /// <summary>
    /// A pubsub node of the "collection" type.
    /// </summary>
    [XmppMember("collection")]
    Collection,

    /// <summary>
    /// A pubsub node of the "leaf" type.
    /// </summary>
    [XmppMember("leaf")]
    Leaf,

    /// <summary>
    /// A personal eventing service that supports the publish-subscribe subset defined in XEP-0163.
    /// </summary>
    [XmppMember("pep")]
    Owned,

    /// <summary>
    /// A pubsub service that supports the functionality defined in XEP-0060.
    /// </summary>
    [XmppMember("service")]
    Service
}
