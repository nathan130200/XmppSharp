using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The headline category consists of services that provide real-time news or information (often but not necessarily in a message of type "headline").
/// </summary>
[XmppEnum]
public enum HeadlineValues
{
	/// <summary>
	/// Service that notifies a user of new email messages.
	/// </summary>
	[XmppMember("newmail")]
	NewMail,

	/// <summary>
	/// RSS notification service.
	/// </summary>
	[XmppMember("rss")]
	RSS,

	/// <summary>
	/// Service that provides weather alerts.
	/// </summary>
	[XmppMember("weather")]
	Weather
}