namespace XmppSharp.Protocol.Extensions.XEP0030;

/// <summary>
/// Service discovery identities.
/// </summary>
public static partial class Identities
{
	/// <summary>
	/// The "account" category is to be used by a server when responding to a disco request sent to the bare JID (<c>user@host</c> addresss) of an account hosted by the server.
	/// </summary>
	public static class Account
	{
		const string Name = "account";

		/// <summary>
		/// The <c>user@host</c> is an administrative account.
		/// </summary>
		public static Identity Admin => new(Name, "admin");

		/// <summary>
		/// The <c>user@host</c> is a "guest" account that allows anonymous login by any user.
		/// </summary>
		public static Identity Anonymous => new(Name, "anonymous");
		/// <summary>
		/// The <c>user@host</c> is a registered or provisioned account associated with a particular non-administrative user
		/// </summary>
		public static Identity Registered => new(Name, "registered");
	}

}