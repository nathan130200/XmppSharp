using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Text;
using System.Web;
using System.Xml;
using XmppSharp.Collections;
using XmppSharp.Dom;

namespace XmppSharp;

public static class Xml
{
    public const string XmppStreamEnd = "</stream:stream>";
    public const string XmppTimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffK";

#if NET6_0

    public static XmlResolver ThrowingResolver { get; } = new ThrowingXmlResolverImpl();

    class ThrowingXmlResolverImpl : XmlResolver
    {
        public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
            => throw new NotSupportedException();

        public override Task<object> GetEntityAsync(Uri absoluteUri, string? role, Type? ofObjectToReturn)
            => throw new NotSupportedException();
    }

#endif

    [ThreadStatic]
    private static string? s_IndentChars;

    [ThreadStatic]
    private static string? s_NewLineChars;

    public static string NewLineChars
    {
        get
        {
            s_NewLineChars ??= "\n";
            return s_NewLineChars;
        }
    }

    public static string IndentChars
    {
        get
        {
            s_IndentChars ??= "  ";
            return s_IndentChars;
        }
    }

    public static string? EncodeName(string? s)
        => XmlConvert.EncodeName(s);

    public static string? Escape(string? s)
        => SecurityElement.Escape(s);

    public static string? EscapeAttribute(string? s)
        => HttpUtility.HtmlAttributeEncode(s);

    public static string? Unescape(string? s)
        => HttpUtility.HtmlDecode(s);

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> callback)
    {
        ThrowHelper.ThrowIfNull(source);
        ThrowHelper.ThrowIfNull(callback);

        if (source.Any())
        {
            foreach (var item in source)
                callback(item);
        }

        return source;
    }

    public static bool FindChild(this Element parent, string tagName, string? namespaceURI, [NotNullWhen(true)] out Element? result)
    {
        ThrowHelper.ThrowIfNull(parent);
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);

        result = parent.Child(tagName, namespaceURI);

        return result != null;
    }

    public static bool FindChild(this Element parent, string tagName, [NotNullWhen(true)] out Element? result)
    {
        ThrowHelper.ThrowIfNull(parent);
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);

        result = parent.Child(tagName);

        return result != null;
    }

    public static Element? Child(this Element parent, Func<Element, bool> predicate)
    {
        ThrowHelper.ThrowIfNull(parent);
        ThrowHelper.ThrowIfNull(predicate);
        return parent.Children().FirstOrDefault(predicate);
    }

    public static IEnumerable<Element> Children(this Element parent, Func<Element, bool> predicate)
    {
        ThrowHelper.ThrowIfNull(parent);
        ThrowHelper.ThrowIfNull(predicate);
        return parent.Children().Where(predicate);
    }

    public static Element C(this Element parent, string tagName, string? namespaceURI = default, object? value = default)
    {
        ThrowHelper.ThrowIfNull(parent);

        var child = ElementFactory.CreateElement(tagName, namespaceURI, parent);
        child.SetValue(value);
        parent.AddChild(child);
        return child;
    }

    public static Element? Up(this Element child)
    {
        ThrowHelper.ThrowIfNull(child);
        return child.Parent;
    }

    public static XmlWriter CreateWriter(TextWriter textWriter, XmlFormatting formatting, Encoding? encoding = default)
    {
        var isFragment = formatting.HasFlag(XmlFormatting.OmitXmlDeclaration);

        return XmlWriter.Create(textWriter, new XmlWriterSettings()
        {
            Indent = formatting.HasFlag(XmlFormatting.Indented),
            IndentChars = IndentChars,
            ConformanceLevel = isFragment ? ConformanceLevel.Fragment : ConformanceLevel.Document,
            CloseOutput = false,
            Encoding = encoding ?? Encoding.UTF8,
            NamespaceHandling = formatting.HasFlag(XmlFormatting.OmitDuplicatedNamespaces)
                ? NamespaceHandling.OmitDuplicates
                : NamespaceHandling.Default,

            OmitXmlDeclaration = formatting.HasFlag(XmlFormatting.OmitXmlDeclaration),
            NewLineChars = NewLineChars,
            NewLineOnAttributes = formatting.HasFlag(XmlFormatting.NewLineOnAttributes),
            DoNotEscapeUriAttributes = formatting.HasFlag(XmlFormatting.DoNotEscapeUriAttributes),
            CheckCharacters = formatting.HasFlag(XmlFormatting.CheckCharacters),
        });
    }

    public static void Remove(this IEnumerable<Element?> e)
    {
        if (e.Any())
        {
            foreach (var item in e)
                item?.Remove();
        }
    }

    public static void WriteTree(Element e, XmlWriter xw)
    {
        var skipAttribute = e.Prefix == null ? "xmlns" : $"xmlns:{e.Prefix}";
        xw.WriteStartElement(e.Prefix, e.LocalName, e.Namespace);

        foreach (var (key, value) in e.Attributes)
        {
            if (key == skipAttribute)
                continue;

            var name = new XmppName(key);

            if (!name.HasPrefix)
                xw.WriteAttributeString(name.LocalName, value);
            else
            {
                xw.WriteAttributeString(name.LocalName, name.Prefix switch
                {
                    "xml" => Namespaces.Xml,
                    "xmlns" => Namespaces.Xmlns,
                    _ => e.GetNamespace(name.Prefix) ?? string.Empty
                }, value);
            }
        }

        foreach (var node in e.Nodes())
        {
            if (node is Element child)
                WriteTree(child, xw);
            else if (node is Text text)
                xw.WriteValue(text.Value);
            else if (node is Comment comment)
                xw.WriteComment(comment.Value);
            else if (node is Cdata cdata)
                xw.WriteCData(cdata.Value);
        }

        xw.WriteEndElement();
    }
}
