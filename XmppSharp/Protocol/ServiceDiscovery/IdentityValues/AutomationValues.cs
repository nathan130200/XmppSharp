using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The automation category consists of entities and nodes that provide automated or programmed interaction.
/// </summary>
[XmppEnum]
public enum AutomationValues
{
	/// <summary>
	/// The node for a list of commands; valid only for the node <see cref="Namespaces.AdHoc"/>.
	/// </summary>
	[XmppMember("command-list")]
	CommandList,

	/// <summary>
	/// A node for a specific command; the <c>node</c> attribute uniquely identifies the command.
	/// </summary>
	[XmppMember("command-node")]
	CommandNode,

	/// <summary>
	/// An entity that supports Jabber-RPC.	
	/// </summary>
	[XmppMember("rpc")]
	Rpc,

	/// <summary>
	/// An entity that supports the SOAP XMPP Binding.
	/// </summary>
	[XmppMember("soap")]
	Soap,

	/// <summary>
	/// An entity that provides automated translation services.
	/// </summary>
	[XmppMember("translation")]
	Translation
}
