using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum MessageType
{
	[XmppEnumMember("chat")]
	Chat,
	
	[XmppEnumMember("error")]
	Error,
	
	[XmppEnumMember("groupchat")]
	GroupChat,
	
	[XmppEnumMember("headline")]
	Headline,
	
	[XmppEnumMember("normal")]
	Normal
}