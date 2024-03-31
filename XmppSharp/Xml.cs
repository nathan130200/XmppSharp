using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmppSharp.Dom;
using XmppSharp.Factory;

namespace XmppSharp;

public readonly record struct XmlQualifiedName(
    bool HasPrefix,
    string LocalName,
    string? Prefix = default);

public static class Xml
{
    public const string StreamEnd = "</stream:stream>";

    public static XmlQualifiedName ExtractQualifiedName(string input)
    {
        Require.NotNullOrEmpty(input);

        var ofs = input.IndexOf(':');

        if (ofs == -1)
            return new(false, input, null);
        else
        {
            var prefix = input[0..ofs];
            var localName = input[(ofs + 1)..];

            if (string.IsNullOrWhiteSpace(localName))
                return new(false, input, null);

            return new(true, localName, prefix);
        }
    }

    internal static (StringBuilder Output, XmlWriter Writer) CreateXmlWriter(bool indent, StringBuilder? output = default)
    {
        output ??= new StringBuilder();

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

    public static Element ToXmppElement(this XElement e)
    {
        var name = e.Name;
        var ns = name.NamespaceName;
        var prefix = e.GetPrefixOfNamespace(name.Namespace);

        if (string.IsNullOrEmpty(ns) && name.LocalName
            is "iq" or "message" or "presence")
        {
            ns = Namespaces.Client;
        }

        var result = ElementFactory.Create(name.LocalName, prefix, ns);

        if (string.IsNullOrEmpty(ns))
            ns = null;

        if (prefix != null)
            result.SetNamespace(prefix, ns);
        else
            result.SetNamespace(ns);

        foreach (var attr in e.Attributes())
        {
            var str = attr.ToString();
            var ofs = str.IndexOf('=');
            var attName = str[0..ofs];
            var attVal = str[(ofs + 2)..^1];
            result._attributes[attName] = attVal;
        }

        foreach (var children in e.Elements())
            result.AddChild(children.ToXmppElement());

        return result;
    }

    public static async Task<Element> ParseFromFileAsync(string fileName, Encoding? encoding = default, int bufferSize = -1)
    {
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            return await ParseFromStreamAsync(stream, encoding, bufferSize);
    }

    public static async Task<Element> ParseFromBufferAsync(byte[] buffer, Encoding? encoding = default, int bufferSize = -1)
    {
        using (var ms = new MemoryStream(buffer))
            return await ParseFromStreamAsync(ms, encoding, bufferSize);
    }

    public static async Task<Element> ParseFromStreamAsync(Stream stream, Encoding? encoding = default, int bufferSize = -1)
    {
        using (var parser = new Parser(encoding, bufferSize))
        {
            parser.Reset(stream);

            var tcs = new TaskCompletionSource<Element>();

            {
                AsyncAction<Element> handler = default;

                handler = e =>
                {
                    parser.OnStreamElement -= handler;
                    tcs.TrySetResult(e);
                    return Task.CompletedTask;
                };

                parser.OnStreamElement += handler;

                while (await parser.ReadAsync())
                    await Task.Delay(0);
            }

            return await tcs.Task;
        }
    }

    internal static void ToStringXml(Element element, XmlWriter writer)
    {
        writer.WriteStartElement(element.Prefix, element.LocalName, element.GetNamespace(element.Prefix));

        foreach (var (name, value) in element.Attributes)
        {
            var result = ExtractQualifiedName(name);

            if (!result.HasPrefix)
                writer.WriteAttributeString(result.LocalName, value);
            else
            {
                var ns = result.Prefix switch
                {
                    "xml" => Namespaces.Xml,
                    "xmlns" => Namespaces.Xmlns,
                    _ => element.GetNamespace(result.Prefix)
                };

                writer.WriteAttributeString(result.LocalName, ns, value);
            }
        }

        if (element.Content != null)
            writer.WriteString(element.Content);

        foreach (var child in element.Children())
            ToStringXml(child, writer);

        writer.WriteEndElement();
    }
}