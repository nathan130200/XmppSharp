using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

public enum MessageType
{
	[XmppEnumMember("normal")]
	Normal,

	[XmppEnumMember("chat")]
	Chat,

	[XmppEnumMember("error")]
	Error,

	[XmppEnumMember("groupchat")]
	GroupChat,

	[XmppEnumMember("headline")]
	Headline
}
