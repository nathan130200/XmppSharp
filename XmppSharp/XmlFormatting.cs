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
	/// <inheritdoc cref="XmlWriterSettings.NamespaceHandling" />
	public NamespaceHandling NamespaceHandling { get; init; }

	/// <inheritdoc cref="XmlWriterSettings.ConformanceLevel" />
	public ConformanceLevel ConformanceLevel { get; init; }

	/// <inheritdoc cref="XmlWriterSettings.OmitXmlDeclaration" />
	public bool OmitXmlDeclaration { get; init; }

	/// <inheritdoc cref="XmlWriterSettings.DoNotEscapeUriAttributes" />
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

	/// <inheritdoc cref="XmlWriterSettings.WriteEndDocumentOnClose" />
	public bool WriteEndDocumentOnClose { get; init; }

	/// <inheritdoc cref="XmlWriterSettings.NewLineHandling" />
	public NewLineHandling NewLineHandling { get; init; }

	/// <inheritdoc cref="XmlWriterSettings.NewLineOnAttributes" />
	public bool NewLineOnAttributes { get; init; }

	/// <inheritdoc cref="XmlWriterSettings.NewLineChars" />
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
		ConformanceLevel = ConformanceLevel.Fragment;

		NamespaceHandling = NamespaceHandling.OmitDuplicates;
		OmitXmlDeclaration = true;

		IndentSize = 0;
		IndentChar = ' ';

		NewLineHandling = NewLineHandling.Replace;
		NewLineOnAttributes = false;
		NewLineChars = Environment.NewLine;
	}

	/// <summary>
	/// Standard XML Formatting outputs the xml string without formatting, using the default rules. Highly recommended for serialization, network, file, etc.
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
