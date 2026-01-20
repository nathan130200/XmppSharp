using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0045;


public enum Role
{
	[XmppEnumMember("none")]
	None = 1,

	[XmppEnumMember("moderator")]
	Moderator,

	[XmppEnumMember("participant")]
	Participant,

	[XmppEnumMember("visitor")]
	Visitor,
}
