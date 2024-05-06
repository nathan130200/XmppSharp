using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace XmppSharp;

public readonly record struct XmlQualifiedName
{
	public bool HasPrefix => !string.IsNullOrWhiteSpace(this.Prefix);
	public string LocalName { get; init; }
	public string? Prefix { get; init; }
}

public static class Xml
{
	public const string XmppStreamEnd = "</stream:stream>";

	public static XmlQualifiedName ExtractQualifiedName(string source)
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

	internal static XmlWriter CreateWriter(StringBuilder output, in XmlFormatting formatting)
	{
		Require.NotNull(output);

		string indentChars = string.Empty;

		if (formatting.IndentSize > 0)
		{
			for (int i = 0; i < formatting.IndentSize; i++)
				indentChars += formatting.IndentChars;
		}

		var settings = new XmlWriterSettings
		{
			Indent = formatting.IndentSize > 0,
			IndentChars = indentChars,
			DoNotEscapeUriAttributes = formatting.DoNotEscapeUriAttributes,
			WriteEndDocumentOnClose = formatting.WriteEndDocumentOnClose,
			NewLineHandling = formatting.NewLineHandling,
			NewLineOnAttributes = formatting.NewLineOnAttributes,
			CheckCharacters = true,
			CloseOutput = true,
			ConformanceLevel = ConformanceLevel.Fragment,
			Encoding = Encoding.UTF8,
			NamespaceHandling = NamespaceHandling.OmitDuplicates,
			OmitXmlDeclaration = true,
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

	public static Element C(this Element parent, string qualifiedName, string namespaceURI = default, object value = default)
	{
		Require.NotNull(parent);
		Require.NotNullOrWhiteSpace(qualifiedName);

		var result = new Element(qualifiedName, namespaceURI);

		if (value != null)
			result.Value = Convert.ToString(value, CultureInfo.InvariantCulture);

		parent.AddChild(result);

		return result;
	}

	public static T SetAttributes<T>(this T element, in Dictionary<string, object> attributes) where T : Element
	{
		Require.NotNull(element);
		Require.NotNull(attributes);

		foreach (var (attName, attVal) in attributes)
			element.SetAttribute(attName, attVal);

		return element;
	}

	public static Element C(this Element parent, string qualifiedName, in Dictionary<string, object> attributes, object value = default)
	{
		Require.NotNull(parent);
		Require.NotNullOrWhiteSpace(qualifiedName);
		Require.NotNull(attributes);

		var child = new Element(qualifiedName);

		if (value != null)
			child.Value = Convert.ToString(value, CultureInfo.InvariantCulture);

		foreach (var (attName, attVal) in attributes)
			child.SetAttribute(attName, attVal);

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

	public static async Task<T> C<T>(this T parent, AsyncFunc<T, Element> factory) where T : Element
	{
		Require.NotNull(parent);
		Require.NotNull(factory);

		var child = await factory(parent);

		if (child != null)
			parent.AddChild(child);

		return parent;
	}

	public static Element Up(this Element child)
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

	public static Element Element(string name, string xmlns = default)
	{
		Require.NotNullOrWhiteSpace(name);

		return new(name, xmlns);
	}

	public static Element Element(string name, in Dictionary<string, object> attributes)
	{
		Require.NotNullOrWhiteSpace(name);

		var result = new Element(name);

		if (attributes != null)
		{
			foreach (var (key, value) in attributes)
				result.SetAttribute(key, value);
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
		else if (rawValue is IFormattable fmt)
			result = fmt.ToString(format, ifp);
		else if (rawValue is IConvertible conv)
			result = conv.ToString(ifp);
		else
			result = rawValue.ToString();

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

	public static T SetAttributeValue<T, TStruct>(this T parent, string name, TStruct? value, string? format = default, IFormatProvider? ifp = default)
		where T : Element
		where TStruct : struct
	{
		Require.NotNull(parent);
		Require.NotNullOrWhiteSpace(name);

		return parent.SetAttributeValue(name, value ?? default, format, ifp);
	}

#if NET7_0_OR_GREATER

	public static T GetAttributeValue<T>(this Element parent, string name, T defaultValue = default, IFormatProvider? ifp = default) where T : IParsable<T>
	{
		Require.NotNull(parent);
		Require.NotNullOrWhiteSpace(name);

		var rawValue = parent.GetAttribute(name);

		if (rawValue == null)
			return defaultValue;

		if (!T.TryParse(rawValue, ifp, out var result))
			result = defaultValue;

		return result;
	}

#else

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

#endif
}