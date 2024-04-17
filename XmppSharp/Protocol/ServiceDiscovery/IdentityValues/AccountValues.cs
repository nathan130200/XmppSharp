using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// This  category is to be used by a server when responding to a disco request sent to the bare <see cref="Jid"/> (<c>user@host</c>) of an account hosted by the server.
/// </summary>
[XmppEnum]
public enum AccountValues
{
	/// <summary>
	/// The <c>user@host</c> is an <b>administrative</b> account.
	/// </summary>
	[XmppMember("admin")]
	Admin,

	/// <summary>
	/// The <c>user@host</c> is a <b>guest</b> account that allows anonymous login by any user.
	/// </summary>
	[XmppMember("anonymous")]
	Anonymous,

	/// <summary>
	/// The <c>user@host</c> is a <b>registered</b> or <b>provisioned</b> account associated with a particular non-administrative user,
	/// </summary>
	[XmppMember("registered")]
	Registered
}
