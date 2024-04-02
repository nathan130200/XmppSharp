using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmppSharp.Dom;
using XmppSharp.Factory;

namespace XmppSharp;

/// <summary>
/// Represents the definition of the XML qualified name.
/// </summary>
/// <param name="HasPrefix">Determines whether the name has a prefix.</param>
/// <param name="LocalName">Gets or sets the local name.</param>
/// <param name="Prefix">Gets or sets the prefix.</param>
public readonly record struct XmlQualifiedName(
    bool HasPrefix,
    string LocalName,
    string? Prefix = default);

/// <summary>
/// Class contains a series of specific uses for manipulating XML.
/// </summary>
public static class Xml
{
    /// <summary>
    /// Global constant string representing the XMPP closing tag.
    /// </summary>
    public const string StreamEnd = "</stream:stream>";

    /// <summary>
    /// Helper function that extracts the local name with XML prefix from the qualified name.
    /// </summary>
    /// <param name="input">String containing XML qualified name.</param>
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

    /// <inheritdoc cref="Element.Element(string, string?, string?)" />
    public static Element Element(string name, string? xmlns = default, string? text = default)
        => new(name, xmlns, text);

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

    /// <summary>
    /// Helper function that converts <see cref="XElement" /> to <see cref="Element" />.
    /// </summary>
    /// <param name="e">Element that will be converted.</param>
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

    /// <summary>
    /// Helper function that parses XML from a file.
    /// </summary>
    /// <param name="fileName">XML file name.</param>
    /// <param name="encoding">Optionally a file encoding (default: <see cref="Encoding.UTF8" />)</param>
    /// <param name="bufferSize">XML parser internal character buffer size (default: <see cref="Parser.DefaultBufferSize" />)</param>
    public static async Task<Element> ParseFromFileAsync(string fileName, Encoding? encoding = default, int bufferSize = -1)
    {
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            return await ParseFromStreamAsync(stream, encoding, bufferSize);
    }

    /// <summary>
    /// Helper function that parses XML from a byte buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="encoding">Optionally a file encoding (default: <see cref="Encoding.UTF8" />)</param>
    /// <param name="bufferSize">XML parser internal character buffer size (default: <see cref="Parser.DefaultBufferSize" />)</param>
    public static async Task<Element> ParseFromBufferAsync(byte[] buffer, Encoding? encoding = default, int bufferSize = -1)
    {
        using (var ms = new MemoryStream(buffer))
            return await ParseFromStreamAsync(ms, encoding, bufferSize);
    }

    /// <summary>
    /// Helper function that parses XML from a stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="encoding">Optionally a file encoding (default: <see cref="Encoding.UTF8" />)</param>
    /// <param name="bufferSize">XML parser internal character buffer size (default: <see cref="Parser.DefaultBufferSize" />)</param>

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