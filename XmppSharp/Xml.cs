using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Text;
using System.Web;
using System.Xml;
using XmppSharp.Dom;

namespace XmppSharp;

public static class Xml
{
    public const string XmppEndTag = "</stream:stream>";
    public const string XmppTimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffK";
    public const string XmppTimestampFormatTemplate = $"{{0:{XmppTimestampFormat}}}";

#pragma warning disable

    [ThreadStatic]
    private static string? s_IndentChars;

    [ThreadStatic]
    private static string? s_NewLineChars;

#pragma warning restore

    public static string NewLineChars => s_NewLineChars ?? "\n";
    public static string IndentChars => s_IndentChars ?? "  ";

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
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(callback);

        if (source.Any())
        {
            foreach (var item in source)
                callback(item);
        }

        return source;
    }

    public static XmppElement? Element(this XmppElement parent, Func<XmppElement, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(predicate);
        return parent.Elements().FirstOrDefault(predicate);
    }

    public static IEnumerable<XmppElement> Elements(this XmppElement parent, Func<XmppElement, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(predicate);
        return parent.Elements().Where(predicate);
    }

    public static bool ExtractQualifiedName(string str, [NotNullWhen(true)] out string? prefix, out string localName)
    {
        var ofs = str.IndexOf(':');

        prefix = default;

        if (ofs > 0)
            prefix = str[0..ofs];

        localName = str[(ofs + 1)..];

        return !string.IsNullOrWhiteSpace(prefix);
    }

    public static XmppElement Element(string tagName, string? xmlns = default, object? value = default)
    {
        var result = XmppElementFactory.Create(tagName, xmlns);
        result.SetValue(value);
        return result;
    }

    public static XmppElement C(this XmppElement parent, string tagName, string? xmlns = default, object? value = default)
    {
        ArgumentNullException.ThrowIfNull(parent);

        if (xmlns == null)
        {
            ExtractQualifiedName(tagName, out var prefix, out _);
            xmlns = parent.GetNamespace(prefix);
        }

        var child = XmppElementFactory.Create(tagName, xmlns, parent);
        child.SetValue(value);
        parent.AddChild(child);
        return child;
    }

    public static XmppElement Up(this XmppElement child)
    {
        ArgumentNullException.ThrowIfNull(child);
        return child.Parent!;
    }

    public static XmlWriter CreateWriter(TextWriter textWriter, XmppFormatting formatting, Encoding? encoding = default)
    {
        var isFragment = formatting.HasFlag(XmppFormatting.OmitXmlDeclaration);

        return XmlWriter.Create(textWriter, new XmlWriterSettings()
        {
            Indent = formatting.HasFlag(XmppFormatting.Indented),
            IndentChars = IndentChars,
            ConformanceLevel = isFragment ? ConformanceLevel.Fragment : ConformanceLevel.Document,
            CloseOutput = false,
            Encoding = encoding ?? Encoding.UTF8,
            NamespaceHandling = formatting.HasFlag(XmppFormatting.OmitDuplicatedNamespaces)
                ? NamespaceHandling.OmitDuplicates
                : NamespaceHandling.Default,

            OmitXmlDeclaration = formatting.HasFlag(XmppFormatting.OmitXmlDeclaration),
            NewLineChars = NewLineChars,
            NewLineOnAttributes = formatting.HasFlag(XmppFormatting.NewLineOnAttributes),
            DoNotEscapeUriAttributes = formatting.HasFlag(XmppFormatting.DoNotEscapeUriAttributes),
            CheckCharacters = formatting.HasFlag(XmppFormatting.CheckCharacters),
        });
    }

    public static void Remove(this IEnumerable<XmppElement?>? e)
    {
        if (e?.Any() == true)
        {
            foreach (var item in e)
                item?.Remove();
        }
    }

    public static void WriteTree(XmppElement e, XmlWriter xw)
    {
        xw.WriteStartElement(e.Prefix, e.LocalName, e.Namespace);

        foreach (var (key, value) in e.Attributes)
        {
            var hasPrefix = ExtractQualifiedName(key, out var prefix, out var localName);

            if (!hasPrefix)
                xw.WriteAttributeString(localName, value);
            else
            {
                xw.WriteAttributeString(localName, prefix switch
                {
                    "xml" => Namespaces.Xml,
                    "xmlns" => Namespaces.Xmlns,
                    _ => e.GetNamespace(prefix) ?? string.Empty
                }, value);
            }
        }

        foreach (var node in e.Nodes())
        {
            if (node is XmppElement child)
                WriteTree(child, xw);
            else if (node is XmppText text)
                xw.WriteValue(text.Value);
            //else if (node is XmppComment comment)
            //    xw.WriteComment(comment.Value);
            else if (node is XmppCdata cdata)
                xw.WriteCData(cdata.Value);
        }

        xw.WriteEndElement();
    }
}
