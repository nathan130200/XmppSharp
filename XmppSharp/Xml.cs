using System.Text;
using System.Xml;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp;

public static class Xml
{
    public static (bool HasPrefix, string LocalName, string Prefix) ExtractQName(string input)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);

        var ofs = input.IndexOf(':');

        if (ofs == -1)
            return (false, input, null);
        else
        {
            var prefix = input[0..ofs];
            var localName = input[(ofs + 1)..];

            if (string.IsNullOrWhiteSpace(localName))
                return (false, input, null);

            return (true, prefix, localName);
        }
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

    internal static void WriteXmlTree(Element element, XmlWriter writer)
    {
        writer.WriteStartElement(element.TagName, element.GetNamespace(element.Prefix));

        foreach (var (name, value) in element.Attributes)
        {
            var result = ExtractQName(name);

            if (!result.HasPrefix)
                writer.WriteAttributeString(result.LocalName, value);
            else
            {
                var ns = result.Prefix switch
                {
                    "xml" => Namespace.Xml,
                    "xmlns" => Namespace.Xmlns,
                    _ => element.GetNamespace(result.Prefix)
                };

                writer.WriteAttributeString(result.LocalName, ns, value);
            }
        }

        if (element.Value != null)
            writer.WriteString(element.Value);

        foreach (var child in element.Children())
            WriteXmlTree(child, writer);

        writer.WriteEndElement();
    }
}