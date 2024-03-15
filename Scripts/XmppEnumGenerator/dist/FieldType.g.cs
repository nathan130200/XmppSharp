using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppEnum]
public enum FieldType
{
	[XmppEnumMember("boolean")]
	Boolean,
	
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
	TextSingle
}