using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0085;

[Flags]
public enum ChatStateType
{
	[XmppEnumMember("active")]
	Active = 1 << 0,

	[XmppEnumMember("composing")]
	Composing = 1 << 1,

	[XmppEnumMember("paused")]
	Paused = 1 << 2,

	[XmppEnumMember("inactive")]
	Inactive = 1 << 3,

	[XmppEnumMember("gone")]
	Gone = 1 << 4,
}
