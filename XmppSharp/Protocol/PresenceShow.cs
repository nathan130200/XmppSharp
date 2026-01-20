using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

public enum PresenceShow
{
	[XmppEnumMember("away")]
	Away = 1,

	[XmppEnumMember("chat")]
	Chat,

	[XmppEnumMember("dnd")]
	DoNotDisturb,

	[XmppEnumMember("xa")]
	ExtendedAway
}
