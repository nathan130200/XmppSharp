using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace XmppSharp.Dom;

public class XmppDocument
{
    public Encoding? Encoding { get; set; } = Encoding.UTF8;
    public XmppElement? RootElement { get; set; }

    public XmppDocument()
    {
    }

    public XmppDocument(XmppElement rootElement)
        => RootElement = rootElement;

    public XmppDocument Parse(string xml)
    {
        using (var reader = new StringReader(xml))
            Load(reader);

        return this;
    }

    public XmppDocument Load(string file, Encoding? encoding = default, int bufferSize = -1)
    {
        encoding ??= Encoding.UTF8;

        if (bufferSize <= 0)
            bufferSize = 4096;

        using (var reader = new StreamReader(file, encoding, true, bufferSize))
            Load(reader);

        return this;
    }

    public XmppDocument Load(Stream stream, Encoding? encoding = default, int bufferSize = -1, bool leaveOpen = true)
    {
        encoding ??= Encoding.UTF8;

        using (var reader = new StreamReader(stream, encoding, true, bufferSize, leaveOpen))
            Load(reader);

        return this;
    }

    public void Load(TextReader textReader)
    {
        Throw.IfNull(textReader);

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

        XmppElement? root = default,
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
                                var elem = XmppElementFactory.Create(reader.Name, reader.LookupNamespace(reader.Prefix), current);

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
                            current?.AddChild(new XmppText(reader.Value));
                            break;

                        case XmlNodeType.Whitespace:
                        case XmlNodeType.ProcessingInstruction:
                            break;

                        case XmlNodeType.Comment:
                            current?.AddChild(new XmppComment(reader.Value));
                            break;

                        case XmlNodeType.CDATA:
                            current?.AddChild(new XmppCdata(reader.Value));
                            break;

                        case XmlNodeType.XmlDeclaration:
                            {
                                var encodingName = reader.GetAttribute("encoding");

                                Encoding = encodingName == null
                                    ? Encoding.UTF8
                                    : Encoding.GetEncoding(encodingName);
                            }
                            break;
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
        var formatting = XmppFormatting.OmitDuplicatedNamespaces | XmppFormatting.CheckCharacters;

        if (indented)
            formatting |= XmppFormatting.Indented;

        return ToString(formatting);
    }

    public string ToString(XmppFormatting formatting)
    {
        var sb = new StringBuilder();

        Encoding ??= Encoding.UTF8;

        using (var sw = new StringWriterWithEncoding(sb, Encoding))
        {
            using (var writer = Xml.CreateWriter(sw, formatting, Encoding))
            {
                var writeXmlDecl = !formatting.HasFlag(XmppFormatting.OmitXmlDeclaration);

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
