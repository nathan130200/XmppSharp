using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmppSharp.Dom;

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

    public static byte[] GetBytes(this XElement element)
    {
        Require.NotNull(element);
        return element.ToString(false).GetBytes();
    }

    public static Task<XElement> ParseFromString(string xml, Encoding encoding = default, int bufferSize = -1)
        => ParseFromBuffer(xml.GetBytes(), encoding, bufferSize);

    public static async Task<XElement> ParseFromBuffer(byte[] buffer, Encoding encoding = default, int bufferSize = -1)
    {
        using (var ms = new MemoryStream(buffer))
            return await ParseFromStream(ms, encoding, bufferSize);
    }

    public static async Task<XElement> ParseFromFile(string path, Encoding encoding = default, int bufferSize = -1)
    {
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            return await ParseFromStream(fs, encoding, bufferSize);
    }

    public static async Task<XElement> ParseFromStream(Stream stream, Encoding encoding = default, int bufferSize = -1)
    {
        using var parser = new Parser(encoding, bufferSize);

        var tcs = new TaskCompletionSource<XElement>();

        AsyncAction<XElement> handler = default;

        handler = async e =>
        {
            await Task.Yield();
            tcs.TrySetResult(e);
            parser.OnStreamElement -= handler;
            parser.Dispose();
        };

        parser.OnStreamElement += handler;
        parser.Reset(stream);

        _ = Task.Run(async () =>
        {
            while (await parser.Advance())
                await Task.Delay(1);
        });

        return await tcs.Task;
    }

    public static string ToString(this XElement element, bool indented, char indentChar = ' ', int indentSize = 2)
    {
        Require.NotNull(element);

        using (StringBuilderPool.Rent(out var sb))
        {
            using (var writer = CreateWriter(indented, sb, indentChar, indentSize))
                element.WriteTo(writer);

            return sb.ToString();
        }
    }

    internal static XmlWriter CreateWriter(bool indented, StringBuilder output, char indentChar, int indentSize)
    {
        Require.NotNull(output);

        var settings = new XmlWriterSettings
        {
            Indent = indented,
            IndentChars = new(indentChar, indentSize),
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

    #region Dynamic Get Attributes

    public static IReadOnlyDictionary<string, string> GetAttributes(this XElement element)
    {
        Require.NotNull(element);

        var result = new Dictionary<string, string>();

        foreach (var attr in element.Attributes())
        {
            string key;

            var name = attr.Name;

            if (name.Namespace == XNamespace.None)
                key = name.LocalName;
            else
                key = string.Concat(element.GetPrefixOfNamespace(name.Namespace), ':', name.LocalName);

            result[key] = attr.Value;
        }

        return result;
    }

    /// <summary>
    /// Clear all attributes and descendant nodes from the given element.
    /// </summary>
    public static T Clear<T>(this T element) where T : XElement
    {
        element.Attributes().Where(x => !x.IsNamespaceDeclaration).Remove();
        element.DescendantNodes().Remove();
        return element;
    }

    /// <summary>
    /// Set key/value pair of <paramref name="attributes"/> in the current <paramref name="element"/>
    /// </summary>
    public static T SetAttributes<T>(this T element, in Dictionary<XName, object>? attributes) where T : XElement
    {
        Require.NotNull(element);

        if (attributes != null)
        {
            foreach (var (name, value) in attributes)
                element.SetAttribute(name, value ?? string.Empty);
        }

        return element;
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

    #region Get/Set/Remove Tag (XName)

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

    #region Get/Set/Remove Attribute Helpers

    public static string? GetAttribute(this XElement element, XName name)
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

    public static T RemoveAttribute<T>(this T element, XName name) where T : XElement
    {
        Require.NotNull(element);
        Require.NotNull(name);

        element.Attribute(name)?.Remove();

        return element;
    }

    public static T SetAttribute<T>(this T element, XName name, object? value) where T : XElement
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

        return element;
    }

    #endregion

    #region Get/Set Namespace Helper

    public static XNamespace GetNamespace(this XElement element)
    {
        Require.NotNull(element);

        var ns = element.GetDefaultNamespace();

        if (ns == XNamespace.None)
            return element.GetAttribute("xmlns") ?? XNamespace.None;

        return ns;
    }

    public static void SetNamespace(this XElement element, string xmlns)
    {
        Require.NotNull(element);
        Require.NotNull(xmlns);

        var ns = XNamespace.Get(xmlns);
        var prefix = element.GetPrefixOfNamespace(element.Name.Namespace);

        element.DescendantsAndSelf().ForEach(e =>
        {
            var srcPrefix = e.GetPrefixOfNamespace(e.Name.Namespace);

            if (srcPrefix == prefix)
                e.Name = ns + e.Name.LocalName;

            if (prefix == null)
                e.SetAttribute("xmlns", ns);
            else
                e.SetAttribute(XNamespace.Xmlns + prefix, ns);
        });
    }

    public static XCData Cdata(string value) => new(value);
    public static XComment Comment(string value) => new(value);
    public static XText Text(string value) => new(value);

    public static XAttribute Attr(XName name, object value) => new(name, value);

    public static XAttribute Attr(XName name, object value, string format = default, IFormatProvider? ifp = default)
    {
        ifp ??= CultureInfo.InvariantCulture;

        string attrValue;

        using (StringBuilderPool.Rent(out var sb))
        {
            sb.Append("{0");

            if (!string.IsNullOrEmpty(format))
                sb.Append(':').Append(format);

            sb.Append('}');

            attrValue = string.Format(ifp, sb.ToString(), value);
        }

        return new XAttribute(name, attrValue);
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

    #region Fluent API

    /// <summary>
    /// Helper function to get the qualified name of the element.
    /// </summary>
    public static string GetName(this XElement element)
    {
        Require.NotNull(element);

        var name = element.Name;

        var prefix = element.GetPrefixOfNamespace(name.Namespace);

        if (prefix == null)
            return name.LocalName;

        return string.Concat(prefix, ':', name.LocalName);
    }

    /// <summary>
    /// Create a new element with respective <paramref name="name"/>, 
    /// optionally with a <paramref name="value"/> content 
    /// and with specified <paramref name="attributes"/>.
    /// </summary>
    public static XElement Element(XName name, object? value = default, in Dictionary<XName, object>? attributes = default)
    {
        Require.NotNull(name);

        var element = new XElement(name)
            .SetAttributes(attributes);

        if (value != null)
            element.SetValue(value);

        return element;
    }

    /// <summary>
    /// Gets the parent element or itself if it is the root element.
    /// </summary>
    public static XElement Up(this XElement child)
    {
        Require.NotNull(child);
        return child.Parent ?? child;
    }

    /// <summary>
    /// Determines whether the given element is the root element of the XML tree.
    /// </summary>
    public static bool IsRoot(this XElement element)
        => element.Parent is null;

    /// <summary>Gets the root element from the current <paramref name="node"/>, or <see langword="null" /> if the node is detached from the XML tree.</summary>
    public static XElement Root(this XNode node)
    {
        Require.NotNull(node);

        XElement parent = default;

        while (node.Parent != null)
        {
            parent = node.Parent;
            node = node.Parent;
        }

        return parent;
    }

    /// <summary>Gets the root element from the current node and cast to <typeparamref name="TRoot"/>.</summary>
    public static TRoot Root<TRoot>(this XElement child) where TRoot : XElement
        => child.Root() as TRoot;

    #region Set/Get Attr Typed

#if NET7_0_OR_GREATER

    public static T GetAttr<T>(this XElement element, XName name, T defaultValue = default, IFormatProvider provider = default) where T : IParsable<T>
    {
        Require.NotNull(element);
        Require.NotNull(name);

        var value = element.GetAttribute(name);

        if (T.TryParse(value, provider ?? CultureInfo.InvariantCulture, out var result))
            return result;

        return defaultValue;
    }

#endif

    public static T GetAttr<T>(this XElement element, XName name, TryParseDelegate<T>? converter, T defaultValue = default)
    {
        Require.NotNull(element);
        Require.NotNull(name);

        converter ??= TryParseHelpers.GetConverter(typeof(T)) as TryParseDelegate<T>;
        Require.NotNull(converter);

        if (element.HasAttribute(name))
        {
            var val = element.GetAttribute(name);

            if (val != null && converter(val, out T result))
                return result;
        }

        return defaultValue;
    }

    // Nullable<T>
    public static TElement SetAttr<TElement, TValue>(this TElement element, XName name, TValue? value, string? format = default, IFormatProvider? ifp = default)
        where TElement : XElement where TValue : struct
    {
        Require.NotNull(element);
        Require.NotNull(name);

        return element.SetAttr(name, rawValue: value.GetValueOrDefault(), format, ifp);
    }

    // raw object
    public static TElement SetAttr<TElement>(this TElement element, XName name, object rawValue, string? format = default, IFormatProvider? ifp = default)
        where TElement : XElement
    {
        Require.NotNull(element);
        Require.NotNull(name);

        ifp ??= CultureInfo.InvariantCulture;

        if (rawValue is IFormattable fmt)
            element.SetAttribute(name, fmt.ToString(format, ifp));
        else if (rawValue is IConvertible conv)
            element.SetAttribute(name, conv.ToString(ifp));
        else
            element.SetAttribute(name, rawValue ?? string.Empty);

        return element;
    }

    #endregion

    #region Add Nodes to Parent

    public static TParent C<TParent>(this TParent parent, XElement child) where TParent : XElement
    {
        Require.NotNull(parent);
        Require.NotNull(child);

        parent.Add(child);

        return parent;
    }

    public static TElement Text<TElement>(this TElement element, string value) where TElement : XElement
    {
        Require.NotNull(element);
        element.Add(new XText(value));
        return element;
    }

    public static TElement Comment<TElement>(this TElement element, string value) where TElement : XElement
    {
        Require.NotNull(element);
        element.Add(new XComment(value));
        return element;
    }

    public static TElement Cdata<TElement>(this TElement element, string value) where TElement : XElement
    {
        Require.NotNull(element);
        element.Add(new XCData(value));
        return element;
    }

    public static XElement C(this XElement parent, string localName, params object[] content)
    {
        Require.NotNull(parent);
        Require.NotNullOrWhiteSpace(localName);

        var child = new XElement(parent.GetNamespace() + localName, content);
        parent.Add(child);
        return child;
    }

    public static XElement C(this XElement parent, XName name, params object[] content)
    {
        Require.NotNull(parent);
        Require.NotNull(name);

        var child = new XElement(name, content);
        parent.Add(child);
        return child;
    }

    #endregion

    #endregion
}