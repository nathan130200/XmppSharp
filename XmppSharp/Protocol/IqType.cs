using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum IqType
{
	[XmppMember("error")]
	Error,
	
	[XmppMember("set")]
	Set,
	
	[XmppMember("get")]
	Get,
	
	[XmppMember("result")]
	Result
}