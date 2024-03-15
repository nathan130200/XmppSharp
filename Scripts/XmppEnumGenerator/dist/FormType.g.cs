using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppEnum]
public enum FormType
{
	[XmppEnumMember("form")]
	Form,
	
	[XmppEnumMember("cancel")]
	Cancel,
	
	[XmppEnumMember("submit")]
	Submit,
	
	[XmppEnumMember("result")]
	Result
}