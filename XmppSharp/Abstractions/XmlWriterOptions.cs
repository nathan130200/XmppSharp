using System.Text;
using System.Xml;

namespace XmppSharp.Abstractions;

public static class XmlWriterOptions
{
	const string DEFAULT_INDENT_CHARS = "  ";

	static readonly Encoding s_Utf8NotBOM = new UTF8Encoding(false, true);

	static readonly ThreadLocal<ConformanceLevel> s_ConformanceLevel = new(() => ConformanceLevel.Fragment);
	static readonly ThreadLocal<NamespaceHandling> s_NamespaceHandling = new(() => NamespaceHandling.OmitDuplicates);
	static readonly ThreadLocal<bool> s_CheckCharacters = new(() => true);
	static readonly ThreadLocal<bool> s_OmitXmlDeclaration = new(() => true);
	static readonly ThreadLocal<string> s_IndentChars = new();
	static readonly AsyncLocal<Encoding> s_OutputEncoding = new();
	static readonly ThreadLocal<string> s_NewLineChars = new();
	static readonly ThreadLocal<bool> s_NewLineOnAttributes = new(() => false);

	public static ConformanceLevel ConformanceLevel
	{
		get => s_ConformanceLevel.Value;
		set => s_ConformanceLevel.Value = value;
	}

	public static bool CheckCharacters
	{
		get => s_CheckCharacters.Value;
		set => s_CheckCharacters.Value = value;
	}

	public static NamespaceHandling NamespaceHandling
	{
		get => s_NamespaceHandling.Value;
		set => s_NamespaceHandling.Value = value;
	}

	public static bool OmitXmlDeclaration
	{
		get => s_OmitXmlDeclaration.Value;
		set => s_OmitXmlDeclaration.Value = value;
	}

	public static string IndentChars
	{
		get => s_IndentChars.Value ?? DEFAULT_INDENT_CHARS;
		set => s_IndentChars.Value = value;
	}

	public static Encoding OutputEncoding
	{
		get => s_OutputEncoding.Value ?? s_Utf8NotBOM;
		set => s_OutputEncoding.Value = value;
	}

	public static string NewLineChars
	{
		get => s_NewLineChars.Value ?? "\n";
		set => s_NewLineChars.Value = value;
	}

	public static bool NewLineOnAttributes
	{
		get => s_NewLineOnAttributes.Value;
		set => s_NewLineOnAttributes.Value = value;
	}
}

