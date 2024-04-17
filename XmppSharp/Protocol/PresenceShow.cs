using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum PresenceShow
{
	/// <summary>
	/// The entity is assumed to be online.
	/// </summary>
	Online,

	/// <summary>
	/// The entity or resource is temporarily away.
	/// </summary>
	[XmppMember("away")]
	Away,

	/// <summary>
	/// The entity or resource is actively interested in chatting.
	/// </summary>
	[XmppMember("chat")]
	Chat,

	/// <summary>
	/// The entity or resource is busy.
	/// </summary>
	[XmppMember("dnd")]
	DoNotDisturb,

	/// <summary>
	/// The entity or resource is away for an extended period.
	/// </summary>
	[XmppMember("xa")]
	ExtendedAway
}