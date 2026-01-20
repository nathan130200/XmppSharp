using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0045;


public enum Affiliation
{
	[XmppEnumMember("none")]
	None = 1,

	[XmppEnumMember("outcast")]
	Outcast,

	[XmppEnumMember("member")]
	Member,

	[XmppEnumMember("admin")]
	Admin,

	[XmppEnumMember("owner")]
	Owner,
}
