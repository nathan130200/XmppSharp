namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
	/// <summary>
	/// The "conference" category consists of online conference services such as multi-user chatroom services.
	/// </summary>
	public static class Conference
	{
		const string Name = "conference";

		/// <summary>
		/// Internet Relay Chat service.
		/// </summary>
		public static Identity Irc => new(Name, "irc");

		/// <summary>
		/// Text conferencing service.
		/// </summary>
		public static Identity Text => new(Name, "text");
	}
}