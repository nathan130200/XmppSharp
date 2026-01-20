namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
	/// <summary>
	/// The "collaboration" category consists of services that enable multiple individuals to work together in real time.
	/// </summary>
	public static class Collaboration
	{
		const string Name = "collaboration";

		/// <summary>
		/// Multi-user whiteboarding service.
		/// </summary>
		public static Identity Whiteboard => new(Name, "whiteboard");
	}
}