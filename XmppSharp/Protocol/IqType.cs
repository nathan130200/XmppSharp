using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// Describes the valid IQ types.
/// </summary>
public enum IqType
{
	/// <summary>
	/// The stanza reports an error that has occurred regarding processing or delivery of a request.
	/// </summary>
	[XmppEnumMember("error")]
	Error,

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
	/// The stanza is a response to a successful request.
	/// </summary>
	[XmppEnumMember("result")]
	Result,
}