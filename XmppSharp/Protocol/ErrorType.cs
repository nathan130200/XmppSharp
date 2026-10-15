using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// Describes types of stanza errors.
/// </summary>
public enum ErrorType
{
	/// <summary>
	/// Retry after providing credentials.
	/// </summary>
	[XmppEnumMember("auth")]
	Auth = 1,

	/// <summary>
	/// Do not retry (the error cannot be remedied).
	/// </summary>
	[XmppEnumMember("cancel")]
	Cancel,

	/// <summary>
	/// Proceed (the condition was only a warning).
	/// </summary>
	[XmppEnumMember("continue")]
	Continue,

	/// <summary>
	/// Retry after changing the data sent.
	/// </summary>
	[XmppEnumMember("modify")]
	Modify,

	/// <summary>
	/// Retry after waiting (the error is temporary).
	/// </summary>
	[XmppEnumMember("wait")]
	Wait
}
