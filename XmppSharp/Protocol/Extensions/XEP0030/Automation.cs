namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
	/// <summary>
	/// The "automation" category consists of entities and nodes that provide automated or programmed interaction.
	/// </summary>
	public static class Automation
	{
		const string Name = "automation";

		/// <summary>
		/// The node for a list of commands; valid only for the node <c>http://jabber.org/protocol/commands</c>.
		/// </summary>
		public static Identity CommandList => new(Name, "command-list");

		/// <summary>
		/// A node for a specific command; the "node" attribute uniquely identifies the command.
		/// </summary>
		public static Identity CommandNode => new(Name, "command-node");

		/// <summary>
		/// An entity that supports Jabber-RPC.
		/// </summary>
		public static Identity Rpc => new(Name, "rpc");

		/// <summary>
		/// An entity that supports the SOAP XMPP Binding.
		/// </summary>
		public static Identity Soap => new(Name, "soap");

		/// <summary>
		/// An entity that provides automated translation services.	
		/// </summary>
		public static Identity Translation => new(Name, "translation");
	}
}