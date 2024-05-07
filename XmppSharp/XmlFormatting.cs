using System.Diagnostics;
using System.Xml;

namespace XmppSharp;

/// <summary>
/// Controls how to convert XML nodes to string. You can choose to use the ready-made configurations or extend them according to your needs or context.
/// </summary>
/// <remarks>
/// As it is an immutable struct, you can use the <see langword="with" /> operator to extend an existing configuration.
/// </remarks>
public readonly struct XmlFormatting
{
	/// <summary>
	/// Determines whether to omit <c>xmlns</c> declarations in child elements.
	/// </summary>
	public bool OmitDuplicatedNamespaces { get; init; }

	/// <summary>
	/// Determines whether to escape the URI attributes. Default: <see langword="false" />.
	/// </summary>
	public bool DoNotEscapeUriAttributes { get; init; }

	/// <summary>
	/// Determines whether to include <see cref="Comment" /> nodes in the XML string representation.
	/// </summary>
	public bool IncludeCommentNodes { get; init; }

	/// <summary>
	/// Determines whether to include <see cref="Cdata"/> nodes in the XML string representation.
	/// </summary>
	public bool IncludeCdataNodes { get; init; }

	/// <summary>
	/// Determines whether to include <see cref="Text"/> nodes in the XML string representation.
	/// </summary>
	public bool IncludeTextNodes { get; init; }

	/// <summary>
	/// Determines the indentation size of the XML. Greater than zero means the XML will be formatted.
	/// </summary>
	public int IndentSize { get; init; }

	/// <summary>
	/// Sets the character used for indentation. It can be a tab character (<c>'\t'</c>) or a space (<c>' '</c>) as long as it complies with XML standards.
	/// </summary>
	public string IndentChars { get; init; }

	/// <summary>
	/// Gets or sets a value that indicates whether the System.Xml.XmlWriter will add closing tags to all unclosed element tags when the <see cref="System.Xml.XmlWriter.Close"/> method is called.
	/// </summary>
	public bool WriteEndDocumentOnClose { get; init; }

	/// <summary>
	/// Gets or sets a value indicating whether to normalize line breaks in the output. (Default: <see cref="NewLineHandling.Replace"/>
	/// </summary>
	public NewLineHandling NewLineHandling { get; init; }

	/// <summary>
	/// Gets or sets a value indicating whether to write attributes on a new line. (Default: <see langword="false" />)
	/// </summary>
	public bool NewLineOnAttributes { get; init; }

	/// <summary>
	/// Gets or sets the character string to use for line breaks. (Default: <see cref="Environment.NewLine"/>)
	/// </summary>
	public string NewLineChars { get; init; }

	/// <summary>
	/// Constructor. Creates an instance of <see cref="XmlFormatting" /> with default values. Basically the same value as <seealso cref="None"/>.
	/// </summary>
	public XmlFormatting()
	{
		IncludeCdataNodes = true;
		IncludeCommentNodes = true;
		IncludeTextNodes = true;

		WriteEndDocumentOnClose = true;
		OmitDuplicatedNamespaces = true;
		DoNotEscapeUriAttributes = false;

		IndentSize = 0;
		IndentChars = " ";

		NewLineHandling = NewLineHandling.Replace;
		NewLineOnAttributes = false;
		NewLineChars = Environment.NewLine;
	}

	/// <summary>
	/// Standard XML Formatting outputs the xml string without formatting, using the default rules. Highly recommended for serialization, network sending, file store, etc.
	/// </summary>
	public static XmlFormatting None { get; } = new();

	/// <summary>
	/// XML formatting that outputs formatted and indented string (recommended for debugging purposes only).
	/// </summary>
	public static XmlFormatting Indented { get; } = None with
	{
		IndentSize = 4,
	};
}
