using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0004;

public enum FieldType
{
	[XmppEnumMember("boolean")]
	Boolean = 1,

	[XmppEnumMember("fixed")]
	Fixed,

	[XmppEnumMember("hidden")]
	Hidden,

	[XmppEnumMember("jid-multi")]
	JidMulti,

	[XmppEnumMember("jid-single")]
	JidSingle,

	[XmppEnumMember("list-multi")]
	ListMulti,

	[XmppEnumMember("list-single")]
	ListSingle,

	[XmppEnumMember("text-multi")]
	TextMulti,

	[XmppEnumMember("text-private")]
	TextPrivate,

	[XmppEnumMember("text-single")]
	TextSingle,
}
