using System.Text;
using System.Xml;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp;

public static class Xml
{
    static string getLocalName(this string s)
        => XmlConvert.EncodeLocalName(s);

    public static (bool HasPrefix, string LocalName, string Prefix) ExtractQName(string input)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);

        var ofs = input.IndexOf(':');

        if (ofs == -1)
            return (false, input.getLocalName(), null);
        else
        {
            var prefix = input[0..ofs];
            var localName = input[(ofs + 1)..];

            if (string.IsNullOrWhiteSpace(localName))
                return (false, input.getLocalName(), null);

            return (true, prefix.getLocalName(), localName.getLocalName());
        }
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> callback)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(callback);

        if (enumerable.Any())
        {
            foreach (var item in enumerable)
                callback(item);
        }

        return enumerable;
    }

    public static string ToString(this Element element, bool indent)
    {
        var (result, writer) = CreateXmlWriter(indent);

        using (writer)
            ToStringInternal(element, writer);

        return result.ToString();
    }

    internal static (StringBuilder Output, XmlWriter Writer) CreateXmlWriter(bool indent)
    {
        var output = new StringBuilder();

        var settings = new XmlWriterSettings
        {
            Indent = indent,
            IndentChars = "  ",
            CheckCharacters = true,
            CloseOutput = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            Encoding = Encoding.UTF8,
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            OmitXmlDeclaration = true,
            NewLineChars = "\n"
        };

        return (output, XmlWriter.Create(new StringWriter(output), settings));
    }

    internal static void ToStringInternal(Element element, XmlWriter writer)
    {
        writer.WriteStartElement(element.TagName, element.GetNamespace(element.Prefix));

        foreach (var (attrName, attrVal) in element.Attributes)
        {
            var result = ExtractQName(attrName);

            if (!result.HasPrefix)
                writer.WriteAttributeString(result.LocalName, attrVal);
            else
            {
                var ns = result.Prefix switch
                {
                    "xml" => Namespace.Xml,
                    "xmlns" => Namespace.Xmlns,
                    _ => element.GetNamespace(result.Prefix)
                };

                writer.WriteAttributeString(result.LocalName, ns, attrVal);
            }
        }

        if (element.Value != null)
            writer.WriteString(element.Value);

        foreach (var child in element.Children())
            ToStringInternal(child, writer);

        writer.WriteEndElement();
    }
}