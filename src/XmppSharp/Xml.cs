using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using Jabber.Net;

namespace Jabber;

public static partial class Xml
{
    #region XML Logging & Serializer

    public static event Action<XmppSession, string> OnReadXml;
    public static event Action<XmppSession, string, Exception?> OnWriteXml;

    static string ConvertToXml(object data)
    {
        string? xml;

        if (data is XmlNode node)
            xml = node.ToXml(true);
        else if (data is string str)
            xml = str;
        else
            xml = data?.ToString();

        Debug.Assert(xml != null);

        return xml;
    }

    internal static void FireOnReadXml(this XmppSession session, object param)
        => OnReadXml?.Invoke(session, ConvertToXml(param));

    internal static void FireOnWriteXml(this XmppSession session, object param, Exception? error = default)
        => OnWriteXml?.Invoke(session, ConvertToXml(param), error);

    public static byte[] GetBytes(this string s, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetBytes(s);

    static readonly XmlWriterSettings DefaultXmlWritterSettings = new()
    {
        Indent = false,
        IndentChars = " ",
        Async = false,
        CloseOutput = true,
        OmitXmlDeclaration = true,
        NamespaceHandling = NamespaceHandling.OmitDuplicates,
        NewLineChars = "\n",
        Encoding = Encoding.UTF8,
    };

    static readonly XmlWriterSettings IndentedXmlWritterSettings = new()
    {
        Indent = true,
        IndentChars = " ",
        Async = false,
        CloseOutput = true,
        OmitXmlDeclaration = true,
        NamespaceHandling = NamespaceHandling.OmitDuplicates,
        NewLineChars = "\n",
        Encoding = Encoding.UTF8,
    };

    public static string ToXml(this XmlNode node, bool indent = false)
    {
        using var sb = StringBuilderPool.Rent();

        using (var sw = new StringWriter(sb))
        {
            var settings = indent ? IndentedXmlWritterSettings : DefaultXmlWritterSettings;

            using (var xw = XmlWriter.Create(sw, settings))
                node.WriteTo(xw);

            sw.Flush();
        }

        return sb.ToString();
    }

    #endregion

    public static string StartTag(this XmlElement e)
    {
        using (var scope = StringBuilderPool.Rent($"<{e.Name}"))
        {
            var sb = scope.Value;

            foreach (XmlAttribute att in e.Attributes)
                sb.Append(' ').Append(att.Name).Append("=\"").Append(att.Value).Append('"');

            return sb.Append('>').ToString();
        }
    }

    #region Fluent XML Methods

    public static XmlElement Element(string name, string? xmlns = default, XmlDocument? document = default)
        => (document ?? new XmlDocument()).CreateElement(name, xmlns);

    public static XmlElement C(this XmlElement e, string name, string? xmlns = default)
    {
        var child = e.OwnerDocument.CreateElement(name, xmlns ?? e.NamespaceURI);
        e.AppendChild(child);
        return child;
    }

    public static XmlElement C(this XmlElement parent, XmlElement child)
    {
        if (parent.OwnerDocument != child.OwnerDocument)
        {
            var el = (XmlElement)parent.OwnerDocument.ImportNode(child, true);
            parent.AppendChild(el);
            return el;
        }
        else
        {
            parent.AppendChild(child);
            return child;
        }
    }

    public static XmlElement? GetChild(this XmlElement e, string name, string? ns = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return e.ChildNodes.OfType<XmlElement>()
            .FirstOrDefault(x => x.Name == name
                && (string.IsNullOrEmpty(ns) || x.NamespaceURI == ns));
    }

    public static bool TryGetChild(this XmlElement e, string name, out XmlElement? result)
    {
        result = e.GetChild(name);
        return result != null;
    }

    public static bool TryGetChild(this XmlElement e, string name, string xmlns, out XmlElement? result)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlns);

        result = e.GetChild(name, xmlns);
        return result != null;
    }

    public static XmlElement T(this XmlElement e, string value)
    {
        if (e.PreviousText is XmlText node)
            node.Value += value;
        else
            e.AppendChild(e.OwnerDocument.CreateTextNode(value));

        return e;
    }

    static string GetAttrValue(object? rawValue)
    {
        if (rawValue is null)
            return string.Empty;
        else if (rawValue is IFormattable fmt)
            return fmt.ToString(null, CultureInfo.InvariantCulture);
        else if (rawValue is string s)
            return s;
        else
            return rawValue?.ToString() ?? string.Empty;
    }

    public static XmlElement Attrs(this XmlElement e, params (string name, object value)[] attrs)
    {
        foreach (var (name, value) in attrs)
            e.SetAttribute(name, GetAttrValue(value));

        return e;
    }

    public static XmlElement? Up(this XmlElement e)
    {
        if (e.ParentNode is XmlElement parent)
            return parent;

        return e;
    }

    public static XmlElement Root(this XmlElement e)
    {
        var current = e;

        while (!current.IsRootElement())
            current = (XmlElement)current.ParentNode!;

        return current;

    }

    public static bool IsRootElement(this XmlElement e)
        => e.ParentNode is not XmlElement;

    #endregion
}
