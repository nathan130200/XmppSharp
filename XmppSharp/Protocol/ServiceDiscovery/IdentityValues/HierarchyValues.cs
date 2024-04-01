using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The hierarchy category is used to describe nodes within a hierarchy of nodes; the <see cref="Branch"/> and <see cref="Leaf" /> types are exhaustive.
/// </summary>
[XmppEnum]
public enum HierarchyValues
{
    /// <summary>
    /// A service discovery node that contains further nodes in the hierarchy.
    /// </summary>
    [XmppMember("branch")]
    Branch,

    /// <summary>
    /// A service discovery node that does not contain further nodes in the hierarchy.
    /// </summary>
    [XmppMember("leaf")]
    Leaf
}
