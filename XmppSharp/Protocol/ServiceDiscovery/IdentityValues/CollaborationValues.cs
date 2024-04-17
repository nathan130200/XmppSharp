using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The collaboration category consists of services that enable multiple individuals to work together in real time.
/// </summary>
[XmppEnum]
public enum CollaborationValues
{
	/// <summary>
	/// Multi-user whiteboarding service
	/// </summary>
	[XmppMember("whiteboard")]
	Whiteboard,
}
