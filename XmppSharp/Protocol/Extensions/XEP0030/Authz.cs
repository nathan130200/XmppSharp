namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
	/// <summary>
	/// Services and nodes that provide authorization identities.
	/// </summary>
	public static class Authz
	{
		const string Name = "authz";

		/// <summary>
		/// An authorization service that provides ephemeral identities.
		/// </summary>
		public static Identity Ephemeral => new(Name, "ephemeral");
	}
}