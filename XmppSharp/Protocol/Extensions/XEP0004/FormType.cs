using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0004;


public enum FormType
{
	[XmppEnumMember("cancel")]
	Cancel = 1,

	[XmppEnumMember("form")]
	Prompt,

	[XmppEnumMember("result")]
	Result,

	[XmppEnumMember("submit")]
	Submit,
}