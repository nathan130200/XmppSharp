using XmppSharp.Attributes;

namespace XmppSharp.Protocol;


public enum RosterSubscriptionType
{
	[XmppEnumMember("none")]
	None = 1,

	[XmppEnumMember("remove")]
	Remove,

	[XmppEnumMember("from")]
	From,

	[XmppEnumMember("to")]
	To,

	[XmppEnumMember("both")]
	Both,
}
