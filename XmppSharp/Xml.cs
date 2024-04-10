using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XmppSharp;

public readonly record struct XmlNameInfo(
    bool HasPrefix,
    string LocalName,
    string? Prefix);

public static class Xml
{
    public const string StreamEnd = "</stream:stream>";

    public static XmlNameInfo ExtractQualifiedName(string source)
    {
        Require.NotNullOrWhiteSpace(source);

        var ofs = source.IndexOf(':');
        string prefix = default, localName;

        if (ofs != -1)
            prefix = source[0..ofs];

        localName = source[(ofs + 1)..];

        return new XmlNameInfo(!string.IsNullOrWhiteSpace(prefix), localName, prefix);
    }

    #region Serialization / Deserialization

    public static string ToString(this XElement element, bool indented, char indentChar = ' ', int indentSize = 2)
    {
        Require.NotNull(element);

        using (StringBuilderPool.Rent(out var sb))
        {
            using (var writer = CreateWriter(indented, sb, new(indentChar, indentSize)))
                element.WriteTo(writer);

            return sb.ToString();
        }
    }

    internal static XmlWriter CreateWriter(bool indented, StringBuilder output, string indentChars)
    {
        Require.NotNull(output);

        if (indented)
            Require.NotNullOrWhiteSpace(indentChars);

        var settings = new XmlWriterSettings
        {
            Indent = indented,
            IndentChars = indentChars,
            CheckCharacters = true,
            CloseOutput = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            Encoding = Encoding.UTF8,
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            OmitXmlDeclaration = true,
            NewLineChars = "\n"
        };

        return XmlWriter.Create(new StringWriter(output), settings);
    }

    #endregion

    #region Dynamic Get/Set Attributes

    public static IReadOnlyDictionary<string, string> GetAttributes(this XElement element)
    {
        Require.NotNull(element);

        var result = new Dictionary<string, string>();

        foreach (var attr in element.Attributes())
        {
            string attrName;

            var baseName = attr.Name;

            if (baseName.Namespace == XNamespace.None)
                attrName = baseName.LocalName;
            else
                attrName = string.Concat(element.GetPrefixOfNamespace(baseName.Namespace), ':', baseName.LocalName);

            result[attrName] = attr.Value;
        }

        return result;
    }

    public static T SetAttributes<T>(this T element, Dictionary<XName, object> attributes) where T : XElement
    {
        Require.NotNull(element);
        Require.NotNull(attributes);

        if (attributes != null)
        {
            foreach (var (name, rawValue) in attributes)
            {
                if (rawValue is null)
                {
                    element.SetAttribute(name, string.Empty);
                    continue;
                }

                if (rawValue is ITuple tuple)
                {
                    if (tuple[0] is IFormattable fmt)
                    {
                        string format = tuple.Length >= 2 ? tuple[1] as string : default;
                        IFormatProvider provider = tuple.Length >= 3 ? tuple[2] as IFormatProvider : default;
                        element.SetAttribute(name, fmt.ToString(format, provider));
                        continue;
                    }
                    else if (tuple[0] is IConvertible conv)
                    {
                        element.SetAttribute(name, conv.ToString(CultureInfo.InvariantCulture));
                        continue;
                    }

                    element.SetAttribute(name, rawValue);
                }
            }
        }

        return element;
    }

    #endregion

    #region Fluent API

    public static XElement Element(XName name, object? value = default, Dictionary<XName, object>? attributes = default)
    {
        Require.NotNull(name);

        var result = new XElement(name);

        if (value != null)
            result.SetValue(value);

        if (attributes != null)
            result.SetAttributes(attributes);

        return result;
    }

    public static T C<T>(this XElement parent, Action<T>? callback) where T : XElement, new()
    {
        Require.NotNull(parent);

        var child = new T();
        parent.Add(child);
        callback?.Invoke(child);

        return child;
    }

    public static T C<T>(this XElement parent, T child) where T : XElement, new()
    {
        Require.NotNull(parent);
        Require.NotNull(child);

        parent.Add(child);

        return child;
    }

    public static XElement C(this XElement parent, XName name, object? value = default, Dictionary<XName, object>? attributes = default)
    {
        Require.NotNull(parent);
        Require.NotNull(name);

        var result = Element(name, value, attributes);
        parent.Add(result);
        return result;
    }

    public static XElement C(this XElement parent, string localName, object? value = default, Dictionary<XName, object>? attributes = default)
    {
        Require.NotNull(parent);
        Require.NotNullOrWhiteSpace(localName);

        var result = Element(parent.GetNamespace() + localName, value, attributes);
        parent.Add(result);
        return result;
    }

    public static XElement Up(this XElement child)
    {
        Require.NotNull(child);
        return child.Parent;
    }

    public static XElement Root(this XElement child)
    {
        Require.NotNull(child);

        while (child.Parent != null)
            child = child.Parent;

        return child;
    }

    #endregion

    #region Get/Set/Remvove Tag (String)

    public static bool HasTag(this XElement element, string localName)
    {
        Require.NotNull(element);
        Require.NotNullOrWhiteSpace(localName);
        return element.HasTag(element.GetNamespace() + localName);
    }

    public static string GetTag(this XElement element, string localName)
    {
        Require.NotNull(element);
        Require.NotNullOrWhiteSpace(localName);
        return element.GetTag(element.GetNamespace() + localName);
    }

    public static void RemoveTag(this XElement element, string localName)
    {
        Require.NotNull(element);
        Require.NotNullOrWhiteSpace(localName);

        element.RemoveTag(element.GetNamespace() + localName);
    }

    public static void SetTag(this XElement element, string localName, object value = default)
    {
        Require.NotNull(element);
        Require.NotNullOrWhiteSpace(localName);

        element.SetTag(element.GetNamespace() + localName, value);
    }

    #endregion

    #region Get/Set/Remvove Tag (XName)

    public static bool HasTag(this XElement element, XName name)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        return element.Element(name) != null;
    }

    public static string GetTag(this XElement element, XName name)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        return element.Element(name)?.Value;
    }

    public static void RemoveTag(this XElement element, XName name)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        element.Element(name)?.Remove();
    }

    public static void SetTag(this XElement element, XName name, object value = default)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        var result = new XElement(name);

        element.Add(result);

        if (value != null)
            result.SetValue(value);
    }

    #endregion

    #region Get/Set/Remove Attribute Helper

    public static string GetAttribute(this XElement element, XName name)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        return element.Attribute(name)?.Value;
    }

    public static bool HasAttribute(this XElement element, XName name)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        return element.Attribute(name) != null;
    }

    public static void RemoveAttribute(this XElement element, XName name)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        element.Attribute(name)?.Remove();
    }

    public static void SetAttribute(this XElement element, XName name, object? value)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        var attr = element.Attribute(name);

        if (value is null)
            attr?.Remove();
        else
        {
            if (attr == null)
                element.Add(new XAttribute(name, value));
            else
                attr.SetValue(value);
        }
    }

    #endregion

    #region Get/Set Namespace Helper

    public static XNamespace GetNamespace(this XElement element)
    {
        Require.NotNull(element);

        return element.Name.Namespace;
    }

    public static XNamespace GetNamespace(this XElement element, string prefix)
    {
        Require.NotNull(element);
        Require.NotNullOrWhiteSpace(prefix);

        return element.GetNamespaceOfPrefix(prefix);
    }

    public static void SetNamespace(this XElement element, string xmlns)
    {
        Require.NotNull(element);
        Require.NotNullOrWhiteSpace(xmlns);

        var oldNamespace = element.Name.Namespace;

        element.Descendants().ForEach(n =>
        {
            if (n.Name.Namespace == oldNamespace)
                n.Name = XName.Get(n.Name.LocalName, xmlns);
        });
    }

    #endregion

    #region Get Element (String)

    public static XElement Child(this XElement element, string localName)
    {
        Require.NotNull(element);
        Require.NotNullOrWhiteSpace(localName);

        return element.Element(element.GetNamespace() + localName);
    }

    public static IEnumerable<XElement> Children(this XElement element, string localName)
    {
        Require.NotNull(element);
        Require.NotNullOrWhiteSpace(localName);

        return element.Elements(element.GetNamespace() + localName);
    }

    #endregion

    #region Get Element (Generic)    

    public static T Element<T>(this XElement element) where T : XElement
    {
        Require.NotNull(element);

        return element.Elements<T>().FirstOrDefault();
    }

    public static IEnumerable<T> Elements<T>(this XElement element) where T : XElement
    {
        Require.NotNull(element);

        return element.Elements().OfType<T>();
    }

    #endregion

    #region Try Get Element (String & XName)

    public static bool TryGetChild(this XElement element, string localName, out XElement result)
    {
        Require.NotNull(element);
        Require.NotNullOrWhiteSpace(localName);

        result = element.Element(element.GetNamespace() + localName);
        return result != null;
    }

    public static bool TryGetChild(this XElement element, XName name, out XElement result)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        result = element.Element(name);
        return result != null;
    }

    #endregion
}