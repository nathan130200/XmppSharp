namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
	/// <summary>
	/// The "store" category consists of internal server components that provide data storage and retrieval services.
	/// </summary>
	public static class Store
	{
		const string Name = "store";

		/// <summary>
		/// A server component that stores data in a Berkeley database.
		/// </summary>
		public static Identity Berkeley => new(Name, "berkeley");

		/// <summary>
		/// A server component that stores data on the file system.
		/// </summary>
		public static Identity File => new(Name, "file");

		/// <summary>
		/// A server data storage component other than one of the registered types.
		/// </summary>
		public static Identity Generic => new(Name, "generic");

		/// <summary>
		/// A server component that stores data in an LDAP database.
		/// </summary>
		public static Identity Ldap => new(Name, "ldap");

		/// <summary>
		/// A server component that stores data in a MySQL database
		/// </summary>
		public static Identity Mysql => new(Name, "mysql");

		/// <summary>
		/// A server component that stores data in an Oracle database.
		/// </summary>
		public static Identity Oracle => new(Name, "oracle");

		/// <summary>
		/// A server component that stores data in a PostgreSQL database.
		/// </summary>
		public static Identity Postgres => new(Name, "postgres");
	}
}