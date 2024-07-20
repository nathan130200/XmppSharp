using System.Text;
using System.Xml;

namespace XmppSharp;

public static partial class Xml
{
	public const string XmppStreamEnd = "</stream:stream>";

	public static XmlNameInfo ExtractQualifiedName(string source)
	{
		Require.NotNullOrWhiteSpace(source);

		var ofs = source.IndexOf(':');

		string? prefix = default;

		if (ofs != -1)
			prefix = source[0..ofs];

		var localName = source[(ofs + 1)..];

		return new()
		{
			LocalName = localName,
			Prefix = prefix
		};
	}

	/// <summary>
	/// More information: <a href="https://www.w3.org/TR/xml/#NT-S">https://www.w3.org/TR/xml/#NT-S</a>
	/// </summary>
	public static readonly char[] WhitespaceChars = { ' ', '\r', '\n', '\t' };

	/// <summary>
	/// Removes insignificant whitespaces from the string.
	/// </summary>
	/// <param name="s">String to be changed.</param>
	/// <param name="trimStart">Determines whether to eliminate the insignificant whitespace at the beginning of the string.</param>
	/// <param name="trimEnd">Determines whether to eliminate the insignificant whitespace at the end of the string.</param>
	/// <returns>Returns the string without the insignificant whitespaces.</returns>
	public static string TrimWhitespaces(this string s,
		bool trimStart = true,
		bool trimEnd = true)
	{
		if (trimStart)
			s = s.TrimStart(WhitespaceChars);

		if (trimEnd)
			s = s.TrimEnd(WhitespaceChars);

		return s;
	}

	internal static XmlWriter CreateWriter(StringBuilder output, XmlFormatting formatting)
	{
		Require.NotNull(output);

		var settings = new XmlWriterSettings
		{
			Indent = formatting.IndentSize > 0,
			IndentChars = new string(formatting.IndentChar, formatting.IndentSize),
			DoNotEscapeUriAttributes = formatting.DoNotEscapeUriAttributes,
			WriteEndDocumentOnClose = formatting.WriteEndDocumentOnClose,
			NewLineHandling = formatting.NewLineHandling,
			NewLineOnAttributes = formatting.NewLineOnAttributes,
			CheckCharacters = true,
			CloseOutput = true,
			ConformanceLevel = ConformanceLevel.Fragment,
			Encoding = Encoding.UTF8,
			NamespaceHandling = formatting.NamespaceHandling,
			OmitXmlDeclaration = formatting.OmitXmlDeclaration,
			NewLineChars = formatting.NewLineChars
		};

		return XmlWriter.Create(new StringWriter(output), settings);
	}

	public static void Remove(this IEnumerable<Node> source)
	{
		if (source?.Any() == false)
			return;

		foreach (var item in source!)
			item.Remove();
	}

	public static Element? Up(this Element child)
	{
		Require.NotNull(child);
		return child.Parent;
	}

	public static Element? Root(this Element child)
	{
		while (!child.IsRootElement)
			child = child.Parent!;

		return child!;
	}

	public static Element Element(string name, string? xmlns = default, string? value = default)
	{
		var result = new Element(name, xmlns, value);

		if (value != null)
			result.Value = value;

		return result;
	}

	public static Element C(this Element parent, string name, string? xmlns = default, string? value = default)
	{
		var child = Element(name, xmlns, value);
		parent.AddChild(child);
		return child;
	}
}