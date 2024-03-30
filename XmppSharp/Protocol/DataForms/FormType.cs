using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppEnum]
public enum FormType
{
	[XmppMember("form")]
	Form,
	
	[XmppMember("cancel")]
	Cancel,
	
	[XmppMember("submit")]
	Submit,
	
	[XmppMember("result")]
	Result
}