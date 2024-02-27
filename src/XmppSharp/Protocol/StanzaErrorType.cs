using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// This enum describes the severity of the error and how it can be processed later.
/// </summary>
[BetterEnum]
public enum StanzaErrorType
{
	/// <summary>
	/// Retry after providing credentials.
	/// </summary>
	[XmppEnumMember("auth")]
	Auth,

	/// <summary>
	/// Do not retry (the error cannot be remedied)
	/// </summary>
	[XmppEnumMember("cancel")]
	Cancel,

	/// <summary>
	/// Proceed (the condition was only a warning)
	/// </summary>
	[XmppEnumMember("continue")]
	Continue,

	/// <summary>
	/// Retry after changing the data sent
	/// </summary>
	[XmppEnumMember("modify")]
	Modify,

	/// <summary>
	/// Retry after waiting (the error is temporary)
	/// </summary>
	[XmppEnumMember("wait")]
	Wait
}
