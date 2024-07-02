using System.Globalization;
using System.Text;
using System.Xml;
using XmppSharp.Parser;

namespace XmppSharp;

public static partial class Xml
{
	public const string XmppStreamEnd = "</stream:stream>";

	public static XmlNameInfo ExtractQualifiedName(string source)
	{
		Require.NotNullOrWhiteSpace(source);

		var ofs = source.IndexOf(':');

		string prefix = default;

		if (ofs != -1)
			prefix = source[0..ofs];

		var localName = source[(ofs + 1)..];

		return new()
		{
			LocalName = localName,
			Prefix = prefix
		};
	}

	// More info: https://www.w3.org/TR/xml/#NT-S
	static readonly char[] s_WhitespaceChars = { ' ', '\r', '\n', '\t' };

	public static string TrimWhitespaces(this string s,
		bool trimStart = true,
		bool trimEnd = true)
	{
		if (trimStart)
			s = s.TrimStart(s_WhitespaceChars);

		if (trimEnd)
			s = s.TrimEnd(s_WhitespaceChars);

		return s;
	}

	public static async Task<Element> ParseFromStringAsync(string xml, CancellationToken token = default)
	{
		using (var ms = new MemoryStream(xml.GetBytes()))
		using (var parser = new XmppStreamReader(ms))
		{
			parser.Reset();
			return await parser.GetNextElementAsync(token);
		}
	}

	public static async Task<Element> ParseFromStreamAsync(Stream stream, CancellationToken token = default)
	{
		using (var parser = new XmppStreamReader(stream))
		{
			parser.Reset();
			return await parser.GetNextElementAsync(token);
		}
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
		Require.NotNull(source);

		foreach (var item in source)
			item.Remove();
	}

	public static Element C(this Element parent, string tagName, string value, params (string key, object value)[] attributes)
	{
		var child = parent.C(tagName, attributes);

		if (value != null)
			child.Value = value;

		parent.AddChild(child);

		return child;
	}

	public static Element C(this Element parent, string tagName, params (string key, object value)[] attributes)
	{
		Require.NotNull(parent);
		Require.NotNullOrWhiteSpace(tagName);

		var child = new Element(tagName);

		if (attributes != null)
		{
			foreach (var (attName, attVal) in attributes)
				child.SetAttribute(attName, attVal);
		}

		parent.AddChild(child);

		return child;
	}

	public static T C<T>(this T parent, Element child) where T : Element
	{
		Require.NotNull(parent);
		Require.NotNull(child);

		parent.AddChild(child);

		return parent;
	}

	public static T C<T>(this T parent, Func<T, Element> factory) where T : Element
	{
		Require.NotNull(parent);
		Require.NotNull(factory);

		var child = factory(parent);

		if (child != null)
			parent.AddChild(child);

		return parent;
	}

	public static Element? Up(this Element child)
	{
		Require.NotNull(child);
		return child.Parent;
	}

	public static Element Root(this Element child)
	{
		while (!child.IsRootElement)
			child = child.Parent;

		return child;
	}

	public static Element Element(string name, string value, params (string key, object value)[] attributes)
	{
		var result = Element(name, attributes);

		if (value != null)
			result.Value = value;

		return result;
	}

	public static Element Element(string name, params (string key, object value)[] attributes)
	{
		Require.NotNullOrWhiteSpace(name);

		var result = new Element(name);

		if (attributes != null)
		{
			foreach (var (attName, attVal) in attributes)
				result.SetAttribute(attName, attVal);
		}

		return result;
	}

	public static T SetAttributeValue<T>(this T parent, string name, object? rawValue, string? format = default, IFormatProvider? ifp = default)
		where T : Element
	{
		Require.NotNull(parent);
		Require.NotNullOrWhiteSpace(name);

		ifp ??= CultureInfo.InvariantCulture;

		string result;

		if (rawValue is null)
			result = string.Empty;
		else
		{
			if (format == null)
				result = Convert.ToString(rawValue, ifp);
			else
			{
				try
				{
					result = string.Format($"{{0:{format}}}", rawValue, ifp);
				}
				catch
				{
					result = rawValue.ToString();
				}
			}
		}

		parent.SetAttribute(name, result);

		return parent;
	}

	public static TEnum GetAttributeEnum<TEnum>(this Element element, string name, TEnum defaultValue = default, bool isNumber = false, bool ignoreCase = true, IFormatProvider ifp = default)
		where TEnum : struct, Enum
	{
		Require.NotNull(element);
		Require.NotNullOrWhiteSpace(name);

		ifp ??= CultureInfo.InvariantCulture;

		var temp = element.GetAttribute(name);

		if (isNumber)
		{
			var baseType = Enum.GetUnderlyingType(typeof(TEnum));
			return (TEnum)Convert.ChangeType(temp, baseType, ifp);
		}
		else
		{
			if (Enum.TryParse<TEnum>(temp, ignoreCase, out var result))
				return result;
		}

		return defaultValue;
	}

	public static T SetAttributeEnum<T, TEnum>(this T element, string name, TEnum? value, bool isNumber = false, string? format = default, IFormatProvider? ifp = default)
		where T : Element
		where TEnum : struct, Enum
	{
		Require.NotNull(element);
		Require.NotNullOrWhiteSpace(name);

		var rawValue = value ?? default;

		if (!isNumber)
			return element.SetAttributeValue(name, Convert.ToString(rawValue, ifp));
		else
		{
			var attrVal = Convert.ChangeType(rawValue, Enum.GetUnderlyingType(typeof(TEnum)));
			return element.SetAttributeValue(name, attrVal, format, ifp);
		}
	}

	public static T SetAttributeValue<T, U>(this T parent, string name, U? value, string? format = default, IFormatProvider? ifp = default)
		where T : Element
		where U : struct
	{
		Require.NotNull(parent);
		Require.NotNullOrWhiteSpace(name);

		return parent.SetAttributeValue(name, value ?? default, format, ifp);
	}

	public static T GetAttributeValue<T>(this Element parent, string name, T fallbackValue = default, IFormatProvider? ifp = default)
		where T : IParsable<T>
	{
		Require.NotNull(parent);
		Require.NotNullOrWhiteSpace(name);

		var rawValue = parent.GetAttribute(name);

		if (rawValue == null)
			return fallbackValue;

		if (!T.TryParse(rawValue, ifp, out var result))
			result = fallbackValue;

		return result;
	}

	public static T GetAttributeValue<T>(this Element parent, string name, TryParseDelegate<T>? converter = default, T defaultValue = default)
	{
		Require.NotNull(parent);
		Require.NotNullOrWhiteSpace(name);

		converter ??= TryParseHelpers.GetConverter(typeof(T)) as TryParseDelegate<T>;
		Require.NotNull(converter);

		var value = parent.GetAttribute(name);

		if (value == null)
			return defaultValue;

		if (!converter(value, out var result))
			result = defaultValue;

		return result;
	}
}