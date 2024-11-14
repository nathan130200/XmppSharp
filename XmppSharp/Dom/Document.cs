using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace XmppSharp.Dom;

public sealed class Document
{
    public Encoding Encoding { get; set; } = Encoding.UTF8;
    public Element? RootElement { get; set; }

    public Document()
    {

    }

    public Document(Element rootElement)
        => RootElement = rootElement;

    public Document Parse(string xml)
    {
        using (var reader = new StringReader(xml))
            Load(reader);

        return this;
    }

    public Document Load(string file, Encoding? encoding = default, int bufferSize = -1)
    {
        encoding ??= Encoding.UTF8;

        if (bufferSize <= 0)
            bufferSize = 4096;

        using (var reader = new StreamReader(file, encoding, true, bufferSize))
            Load(reader);

        return this;
    }

    public Document Load(Stream stream, Encoding? encoding = default, int bufferSize = -1, bool leaveOpen = true)
    {
        encoding ??= Encoding.UTF8;

        using (var reader = new StreamReader(stream, encoding, true, bufferSize, leaveOpen))
            Load(reader);

        return this;
    }

    public void Load(TextReader textReader)
    {
        ThrowHelper.ThrowIfNull(textReader);

        var settings = new XmlReaderSettings
        {
            CheckCharacters = true,
            CloseInput = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
            ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes,

#if NET6_0
            XmlResolver = Xml.ThrowingResolver
#else
            XmlResolver = XmlResolver.ThrowingResolver
#endif

        };

        Element? root = default,
            current = default;

        try
        {
            using (var reader = XmlReader.Create(textReader))
            {
                var info = (IXmlLineInfo)reader;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                var elem = ElementFactory.CreateElement(reader.Name, reader.LookupNamespace(reader.Prefix));

                                while (reader.MoveToNextAttribute())
                                    elem.SetAttribute(reader.Name, reader.Value);

                                reader.MoveToElement();

                                if (root == null)
                                    root = elem;

                                if (reader.IsEmptyElement)
                                {
                                    current?.AddChild(elem);
                                }
                                else
                                {
                                    current?.AddChild(elem);
                                    current = elem;
                                }
                            }
                            break;

                        case XmlNodeType.EndElement:
                            {
                                if (current == null)
                                    throw new XmlException("Unexcepted eng tag.", null, info.LineNumber, info.LinePosition);

                                var parent = current?.Parent;

                                if (parent == null)
                                    return;

                                current = parent;
                            }
                            break;

                        case XmlNodeType.SignificantWhitespace:
                        case XmlNodeType.Text:
                            {
                                if (current != null)
                                    current.Value += reader.Value;
                            }
                            break;

                        // skip unwanted whitespaces and PIs
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.ProcessingInstruction:
                            break;

                        case XmlNodeType.XmlDeclaration:
                            {
                                var encodingName = reader.GetAttribute("encoding");

                                Encoding = encodingName == null
                                    ? Encoding.UTF8
                                    : Encoding.GetEncoding(encodingName);
                            }
                            break;

                        default:
                            throw new XmlException($"Unsupported XML token. ({reader.NodeType})", null, info.LineNumber, info.LinePosition);
                    }
                }
            }
        }
        finally
        {
            RootElement = root;
        }
    }

    public override string ToString()
        => ToString(false);

    public string ToString(bool indented)
    {
        var formatting = XmlFormatting.OmitDuplicatedNamespaces | XmlFormatting.CheckCharacters;

        if (indented)
            formatting |= XmlFormatting.Indented;

        return ToString(formatting);
    }

    public string ToString(XmlFormatting formatting)
    {
        var sb = new StringBuilder();

        using (var sw = new StringWriterWithEncoding(sb, Encoding))
        {
            using (var writer = Xml.CreateWriter(sw, formatting, Encoding))
            {
                var writeXmlDecl = !formatting.HasFlag(XmlFormatting.OmitXmlDeclaration);

                if (writeXmlDecl)
                    writer.WriteStartDocument();

                RootElement?.WriteTo(writer);

                if (writeXmlDecl)
                    writer.WriteEndDocument();
            }
        }

        return sb.ToString();
    }
}


public sealed class StringWriterWithEncoding : StringWriter
{
    private readonly Encoding _encoding;

    public StringWriterWithEncoding(StringBuilder @out, Encoding encoding) : base(@out)
    {
        ThrowHelper.ThrowIfNull(encoding);

        _encoding = encoding;
    }

    public override Encoding Encoding
        => _encoding;
}