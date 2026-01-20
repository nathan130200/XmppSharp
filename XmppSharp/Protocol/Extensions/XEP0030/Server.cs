namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
	/// <summary>
	/// The "server" category consists of any Jabber/XMPP server.
	/// </summary>
	public static class Server
	{
		const string Name = "server";

		/// <summary>
		/// Standard Jabber/XMPP server used for instant messaging and presence.
		/// </summary>
		public static Identity IM => new(Name, "im");
	}
}