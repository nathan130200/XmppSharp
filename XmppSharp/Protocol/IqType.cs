using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

public enum IqType
{
	[XmppEnumMember("error")]
	Error = 1,

	[XmppEnumMember("get")]
	Get,

	[XmppEnumMember("set")]
	Set,

	[XmppEnumMember("result")]
	Result
}