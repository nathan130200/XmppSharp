using System.Xml;

namespace XmppSharp.Xml;

/// <summary>
/// A helper class for writing well-formed XML elements.
/// </summary>
public sealed class DomWriter : IDisposable
{
    readonly XmlWriter _writer;

    readonly NamespaceStack _namespaces = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DomWriter"/> class.
    /// </summary>
    /// <param name="output">The output destination for the XML data.</param>
    /// <param name="indent">Indicates whether to indent the XML elements.</param>
    /// <param name="omitXmlDeclaration">Indicates whether to omit the XML declaration.</param>
    /// <param name="closeOutput">Indicates whether to close the output when the writer is disposed.</param>
    /// <param name="writeEndDocumentOnClose">Indicates whether to write the end document when the writer is closed.</param>
    /// <param name="checkCharacters">Indicates whether to check characters for validity.</param>
    /// <param name="conformanceLevel">Specifies the level of conformance for the XML document.</param>
    /// <param name="namespaceHandling">Specifies how namespaces are handled.</param>
    public DomWriter(dynamic output,
        bool indent = false, bool omitXmlDeclaration = true, bool closeOutput = true,
        bool writeEndDocumentOnClose = false, bool checkCharacters = true,
        ConformanceLevel conformanceLevel = ConformanceLevel.Fragment,
        NamespaceHandling namespaceHandling = NamespaceHandling.OmitDuplicates
    )
    {
        _writer = XmlWriter.Create(output, new XmlWriterSettings()
        {
            Indent = indent,
            ConformanceLevel = conformanceLevel,
            NamespaceHandling = namespaceHandling,
            CheckCharacters = checkCharacters,
            CloseOutput = closeOutput,
            OmitXmlDeclaration = omitXmlDeclaration,
            WriteEndDocumentOnClose = writeEndDocumentOnClose
        });
    }

    /// <summary>
    /// Writes the XML declaration at the beginning of the document.
    /// </summary>
    public void WriteStartDocument() => _writer.WriteStartDocument();

    /// <summary>
    /// Writes the end of the XML document, ensuring that all elements are properly closed.
    /// </summary>
    public void WriteEndDocument() => _writer.WriteEndDocument();

    /// <summary>
    /// Begins a new namespace scope.
    /// </summary>
    public void BeginNamespaceScope()
    {
        _namespaces.PushScope();
    }

    /// <summary>
    /// Ends the current namespace scope.
    /// </summary>
    public void EndNamespaceScope()
    {
        _namespaces.PopScope();
    }

    /// <summary>
    /// Adds a namespace declaration to the current scope.
    /// </summary>
    /// <param name="prefix">The prefix to associate with the namespace URI.</param>
    /// <param name="uri">The namespace URI to associate with the prefix.</param>
    public void AppendNamespace(string? prefix, string uri)
    {
        _namespaces.AddNamespace(prefix, uri);
    }

    /// <summary>
    /// Writes the start of an XML element. 
    /// </summary>
    /// <param name="prefix">The prefix of the element.</param>
    /// <param name="localName">The local name of the element.</param>
    /// <param name="ns">The namespace URI of the element. If not provided, it will be looked up based on the prefix.</param>
    public void WriteStartElement(string? prefix, string localName, string? ns = default)
    {
        ns ??= _namespaces.LookupNamespace(prefix);
        _writer.WriteStartElement(prefix, localName, ns);
    }

    /// <summary>
    /// Writes an attribute for the current XML element.
    /// </summary>
    /// <param name="prefix">The prefix of the attribute. If not provided, the attribute will be written without a namespace.</param>
    /// <param name="localName">The local name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    public void WriteAttribute(string? prefix, string localName, string? value)
    {
        if (prefix == null)
            _writer.WriteAttributeString(localName, value);
        else
        {
            var ns = _namespaces.LookupNamespace(prefix);
            _writer.WriteAttributeString(prefix, localName, ns, value);
        }
    }

    /// <summary>
    /// Writes the end of the current XML element.
    /// </summary>
    public void WriteEndElement()
    {
        _writer.WriteEndElement();
    }

    /// <summary>
    /// Writes raw XML content. 
    /// </summary>
    /// <param name="value">The raw XML content to write.</param>
    public void WriteRaw(string value)
        => _writer.WriteRaw(value);

    /// <summary>
    /// Writes text content for the current XML element.
    /// </summary>
    /// <param name="value">The text content to write.</param>
    public void WriteText(string? value)
        => _writer.WriteString(value);

    /// <summary>
    /// Writes a CDATA section for the current XML element.
    /// </summary>
    /// <param name="value">The CDATA content to write.</param>
    public void WriteCdata(string? value)
        => _writer.WriteCData(value);

    /// <summary>
    /// Disposes the writer, ensuring that all resources are released and any pending XML content is flushed to the output.
    /// </summary>
    public void Dispose()
    {
        _writer.Dispose();
    }
}
