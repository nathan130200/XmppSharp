using System.Text;
using System.Xml;
using XmppSharp.Xmpp;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp;

public static class Xml
{
    public const string JabberEndTag = "</stream:stream>";

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

            return (true, localName, prefix);
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

    public static Task<Element> ParseFromFileAsync(string fileName, Encoding? encoding = default, int bufferSize = -1)
    {
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            return ParseFromStreamAsync(stream, encoding, bufferSize);
    }

    public static Task<Element> ParseFromBufferAsync(byte[] buffer, Encoding? encoding = default, int bufferSize = -1)
    {
        using (var ms = new MemoryStream(buffer))
            return ParseFromStreamAsync(ms, encoding, bufferSize);
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

    internal static void WriteXmlTree(Element element, XmlWriter writer)
    {
        writer.WriteStartElement(element.Prefix, element.LocalName, element.GetNamespace(element.Prefix));

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