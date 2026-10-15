using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

public enum ErrorType
{
	[XmppEnumMember("auth")]
	Auth = 1,

	[XmppEnumMember("cancel")]
	Cancel,

	[XmppEnumMember("continue")]
	Continue,

	[XmppEnumMember("modify")]
	Modify,

	[XmppEnumMember("wait")]
	Wait
}
