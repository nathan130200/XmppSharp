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
	/// Sets the character used for indentation. It can be a tab character ('\t') or a space (' ') as long as it complies with XML standards
	/// </summary>
	public char IndentChar { get; init; }

	/// <summary>
	/// Constructor. Creates an instance of <see cref="XmlFormatting" /> with default values. Basically the same value as <seealso cref="Default"/>.
	/// </summary>
	public XmlFormatting()
	{
		OmitDuplicatedNamespaces = true;
		DoNotEscapeUriAttributes = false;
		IncludeCdataNodes = true;
		IncludeCommentNodes = true;
		IncludeTextNodes = true;
		IndentSize = 0;
		IndentChar = ' ';
	}

	/// <summary>
	/// Standard XML Formatting outputs the xml string without formatting, using the default rules. Highly recommended for serialization and network streaming.
	/// </summary>
	public static XmlFormatting Default { get; } = new();

	/// <summary>
	/// XML formatting that outputs formatted and indented string (recommended for debugging purposes only).
	/// </summary>
	public static XmlFormatting Indented { get; } = Default with
	{
		IndentSize = 2,
		IndentChar = ' '
	};

	/// <summary>
	/// XML formatting that does only emit nodes that are of type <see cref="Element" />
	/// </summary>
	public static XmlFormatting ElementsOnly { get; } = Default with
	{
		IncludeCommentNodes = false,
		IncludeCdataNodes = false,
	};
}
