namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
	/// <summary>
	/// The "hierarchy" category is used to describe nodes within a hierarchy of nodes; the "branch" and "leaf" types are exhaustive.
	/// </summary>
	public static class Hierarchy
	{
		const string Name = "hierarchy";

		/// <summary>
		/// A service discovery node that contains further nodes in the hierarchy.
		/// </summary>
		public static Identity Branch => new(Name, "branch");

		/// <summary>
		/// A service discovery node that does not contain further nodes in the hierarchy.
		/// </summary>
		public static Identity Leaf => new(Name, "leaf");
	}
}