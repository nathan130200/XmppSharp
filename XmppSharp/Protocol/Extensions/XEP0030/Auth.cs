namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
	/// <summary>
	/// The "auth" category consists of server components that provide authentication services within a server implementation.
	/// </summary>
	public static class Auth
	{
		const string Name = "auth";

		/// <summary>
		/// A server component that authenticates based on external certificates.
		/// </summary>
		public static Identity Cert => new(Name, "cert");

		/// <summary>
		/// A server authentication component other than one of the registered types.
		/// </summary>
		public static Identity Generic => new(Name, "generic");

		/// <summary>
		/// A server component that authenticates against an LDAP database.
		/// </summary>
		public static Identity Ldap => new(Name, "ldap");

		/// <summary>
		/// A server component that authenticates against an NT domain.
		/// </summary>
		public static Identity Ntlm => new(Name, "ntlm");

		/// <summary>
		/// A server component that authenticates against a PAM system.
		/// </summary>
		public static Identity Pam => new(Name, "pam");

		/// <summary>
		/// A server component that authenticates against a Radius system.
		/// </summary>
		public static Identity Radius => new(Name, "radius");
	}
}