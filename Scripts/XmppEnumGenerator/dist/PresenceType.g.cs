using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum PresenceType
{
	[XmppEnumMember("error")]
	Error,
	
	Available,
	
	[XmppEnumMember("unavailable")]
	Unavailable,
	
	[XmppEnumMember("probe")]
	Probe,
	
	[XmppEnumMember("subscribe")]
	Subscribe,
	
	[XmppEnumMember("subscribed")]
	Subscribed,
	
	[XmppEnumMember("unsubscribe")]
	Unsubscribe,
	
	[XmppEnumMember("unsubscribed")]
	Unsubscribed
}