using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// This enum lists the types of stanzas that can be sent or received and how they will be handled.
/// </summary>
[BetterEnum]
public enum IqType
{
	/// <summary>
	/// The stanza requests information, inquires about what data is needed in order to complete further operations, etc.
	/// </summary>
	[XmppEnumMember("get")]
	Get,

	/// <summary>
	/// The stanza provides data that is needed for an operation to be completed, sets new values, replaces existing values, etc.
	/// </summary>
	[XmppEnumMember("set")]
	Set,

	/// <summary>
	/// The stanza is a response to a successful get or set request.
	/// </summary>
	[XmppEnumMember("result")]
	Result,

	/// <summary>
	/// The stanza reports an error that has occurred regarding processing or delivery of a get or set request.
	/// </summary>
	[XmppEnumMember("error")]
	Error
}
