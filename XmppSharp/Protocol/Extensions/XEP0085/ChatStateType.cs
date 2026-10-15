using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0085;

/// <summary>
/// The possible Chat States.
/// </summary>
[Flags]
public enum ChatStateType
{
	/// <summary>
	/// User is actively participating in the chat session.
	/// </summary>
	[XmppEnumMember("active")]
	Active = 1 << 0,

	/// <summary>
	/// User is composing a message.
	/// </summary>
	[XmppEnumMember("composing")]
	Composing = 1 << 1,

	/// <summary>
	/// User had been composing but now has stopped.
	/// </summary>
	[XmppEnumMember("paused")]
	Paused = 1 << 2,

	/// <summary>
	/// User has not been actively participating in the chat session.
	/// </summary>
	[XmppEnumMember("inactive")]
	Inactive = 1 << 3,

	/// <summary>
	/// User has effectively ended their participation in the chat session.
	/// </summary>
	[XmppEnumMember("gone")]
	Gone = 1 << 4,
}
