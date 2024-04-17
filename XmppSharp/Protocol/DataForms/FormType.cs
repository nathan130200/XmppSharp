using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

/// <summary>
/// Specifies the different types of forms that can be used in XMPP Data Forms.
/// Each type designates a specific purpose or interaction model within a data form exchange.
/// </summary>
[XmppEnum]
public enum FormType
{
	/// <summary>
	/// Denotes a form intended for collecting user input or presenting options.
	/// It typically contains fields for data entry and submission.
	/// </summary>
	[XmppMember("form")]
	Form,

	/// <summary>
	/// Signals a request to cancel an ongoing form interaction.
	/// It might be used to abandon a partially completed form or revert changes.
	/// </summary>
	[XmppMember("cancel")]
	Cancel,

	/// <summary>
	/// Activates the submission of collected data or form results.
	/// It initiates the transmission of completed form data to the server or other entities.
	/// </summary>
	[XmppMember("submit")]
	Submit,

	/// <summary>
	/// Communicates the outcome or result of a data form submission.
	/// It might convey success, failure, or provide confirmation of processed data.
	/// </summary>
	[XmppMember("result")]
	Result
}