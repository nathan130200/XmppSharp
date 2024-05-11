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
	/// Determines the behavior of declaring namespaces in the XML element. (Default: <see cref="NamespaceHandling.OmitDuplicates"/>)
	/// </summary>
	public NamespaceHandling NamespaceHandling { get; init; }

	/// <summary>
	/// Determines whether the XML output will omit the prologue, i.e. <c><![CDATA[<?xml version='1.0' ... ?>]]></c>. (Default: <see langword="true" />)
	/// </summary>
	public bool OmitXmlDeclaration { get; init; }

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
	/// Determines whether the XML string will be formatted. (Default: <see langword="0" />)
	/// </summary>
	public int IndentSize { get; init; }

	/// <summary>
	/// Sets the character used for indentation. (Default: <see cref="string.Empty" />)
	/// </summary>
	public char IndentChar { get; init; }

	/// <summary>
	/// Gets or sets a value that indicates whether the System.Xml.XmlWriter will add closing tags to all unclosed element tags when the <see cref="System.Xml.XmlWriter.Close"/> method is called.
	/// </summary>
	public bool WriteEndDocumentOnClose { get; init; }

	/// <summary>
	/// Gets or sets a value indicating whether to normalize line breaks in the output. (Default: <see cref="NewLineHandling.Replace"/>)
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
		DoNotEscapeUriAttributes = false;

		NamespaceHandling = NamespaceHandling.OmitDuplicates;
		OmitXmlDeclaration = true;

		IndentSize = 0;
		IndentChar = (char)0;

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
		IndentChar = ' '
	};
}
