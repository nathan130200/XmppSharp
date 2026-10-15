using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

public enum MessageType : byte
{
	[XmppEnumMember("error")]
	Error,

	[XmppEnumMember("normal")]
	Normal,

	[XmppEnumMember("headline")]
	Headline,

	[XmppEnumMember("chat")]
	Chat,

	[XmppEnumMember("groupchat")]
	GroupChat
}
