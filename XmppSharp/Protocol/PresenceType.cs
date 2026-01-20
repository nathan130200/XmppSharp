using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

public enum PresenceType
{
	Available,

	[XmppEnumMember("error")]
	Error,

	[XmppEnumMember("probe")]
	Probe,

	[XmppEnumMember("subscribe")]
	Subscribe,

	[XmppEnumMember("subscribed")]
	Subscribed,

	[XmppEnumMember("unavailable")]
	Unavailable,

	[XmppEnumMember("unsubscribe")]
	Unsubscribe,

	[XmppEnumMember("unsubscribed")]
	Unsubscribed,
}
