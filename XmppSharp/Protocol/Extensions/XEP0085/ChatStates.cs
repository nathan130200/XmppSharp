using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0085;


public enum ChatStates
{
	/// <summary>
	/// User is actively participating in the chat session.
	/// </summary>
	[XmppEnumMember("active")]
	Active = 1,

	/// <summary>
	/// User has not been actively participating in the chat session.
	/// </summary>
	[XmppEnumMember("inactive")]
	Inactive,

	/// <summary>
	/// User has effectively ended their participation in the chat session.
	/// </summary>
	[XmppEnumMember("gone")]
	Gone,

	/// <summary>
	/// User is composing a message.
	/// </summary>
	[XmppEnumMember("composing")]
	Composing,

	/// <summary>
	/// User had been composing but now has stopped.
	/// </summary>
	[XmppEnumMember("paused")]
	Paused
}
