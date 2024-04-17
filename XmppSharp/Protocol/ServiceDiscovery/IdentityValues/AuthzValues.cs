using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// Services and nodes that provide authorization identities.
/// </summary>
[XmppEnum]
public enum AuthzValues
{
	/// <summary>
	/// An authorization service that provides ephemeral identities.
	/// </summary>
	[XmppMember("ephemeral")]
	Ephemeral
}