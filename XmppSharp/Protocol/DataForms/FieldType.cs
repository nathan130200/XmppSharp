using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

/// <summary>
/// Specifies the different types of fields that can be used within Data Forms in XMPP.
/// Each type defines the expected format and behavior of a field, enabling the collection of various data types.
/// </summary>
[XmppEnum]
public enum FieldType
{
	/// <summary>
	/// Represents a field that accepts boolean values (true or false), typically using a checkbox for input.
	/// </summary>
	[XmppMember("boolean")]
	Boolean,

	/// <summary>
	/// Represents a field with a fixed, pre-defined value that cannot be changed by the user.
	/// </summary>
	[XmppMember("fixed")]
	Fixed,

	/// <summary>
	/// Represents a field that is hidden from the user interface but still submitted with form data.
	/// </summary>
	[XmppMember("hidden")]
	Hidden,

	/// <summary>
	/// Represents a field that accepts multiple Jabber IDs (JIDs), representing entities within XMPP.
	/// </summary>
	[XmppMember("jid-multi")]
	JidMulti,

	/// <summary>
	/// Represents a field that accepts a single Jabber ID (JID).
	/// </summary>
	[XmppMember("jid-single")]
	JidSingle,

	/// <summary>
	/// Represents a field that allows selection of multiple options from a pre-defined list.
	/// </summary>
	[XmppMember("list-multi")]
	ListMulti,

	/// <summary>
	/// Represents a field that allows selection of a single option from a pre-defined list.
	/// </summary>
	[XmppMember("list-single")]
	ListSingle,

	/// <summary>
	/// Represents a field that accepts multiple lines of free-form text input.
	/// </summary>
	[XmppMember("text-multi")]
	TextMulti,

	/// <summary>
	/// Represents a field that accepts free-form text input, but the value is masked from the user interface.
	/// </summary>
	[XmppMember("text-private")]
	TextPrivate,

	/// <summary>
	/// Represents a field that accepts a single line of free-form text input.
	/// </summary>
	[XmppMember("text-single")]
	TextSingle
}