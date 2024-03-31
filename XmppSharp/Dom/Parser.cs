using System.Text;
using System.Xml;
using System.Xml.Schema;
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Dom;

/// <summary>
/// Represents the class that reads the XMPP from a stream.
/// </summary>
public sealed class Parser : IDisposable
{
    private XmlReader _reader;
    private StreamReader _charStream;
    private NameTable _nameTable = new();
    private volatile bool _disposed;

    private readonly Encoding _encoding;
    private readonly int _bufferSize;

    public const int DefaultBufferSize = 64;

    /// <summary>
    /// Gets or sets whether the parser will throw an exception when reading node types not supported by XMPP. (Default: True)
    /// <para>
    /// Only the following node types are supported by XMPP are:
    /// <list type="bullet">
    /// <item><see cref="XmlNodeType.Element"/></item>
    /// <item><see cref="XmlNodeType.EndElement"/></item>
    /// <item><see cref="XmlNodeType.Attribute"/></item>
    /// <item><see cref="XmlNodeType.XmlDeclaration"/></item>
    /// <item><see cref="XmlNodeType.Text"/></item>
    /// <item><see cref="XmlNodeType.SignificantWhitespace"/></item>
    /// <item><see cref="XmlNodeType.Whitespace"/></item>
    /// </list>
    /// </para>
    /// </summary>
    public bool ThrownOnUnsupportedXmlFeatures { get; set; } = true;

    /// <summary>
    /// Initializes a new <see cref="Parser"/> instance.
    /// </summary>
    /// <param name="encoding">Provides a specific encoding that will be used to read the characters. (Default <see cref="Encoding.UTF8" />)</param>
    /// <param name="bufferSize">Sets the expected size of the character buffer that will be used when reading. (Default: 64 characters)</param>
    public Parser(Encoding? encoding = default, int bufferSize = DefaultBufferSize)
    {
        _encoding = encoding;
        _bufferSize = bufferSize <= 0 ? DefaultBufferSize : bufferSize;
    }

    /// <summary>
    /// The event is triggered when the XMPP open tag is found <c>&lt;stream:stream&gt;</c>
    /// </summary>
    public event AsyncAction<StreamStream> OnStreamStart;

    /// <summary>
    /// The event is triggered when any well-formed element is found.
    /// <para>However, if the XML tag is registered using <see cref="ElementFactory" /> the parser will automatically construct the element in the registered type.</para>
    /// <para>Elements that cannot be constructed using <see cref="ElementFactory" /> only return the type <see cref="Element" />.</para>
    /// </summary>
    public event AsyncAction<Element> OnStreamElement;

    /// <summary>
    /// The event is triggered when the XMPP close tag is found <c>&lt;/stream:stream&gt;</c>
    /// </summary>
    public event AsyncAction OnStreamEnd;

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _charStream?.Dispose();
        _reader?.Dispose();
        _nameTable = null;
    }

#if !NET7_0_OR_GREATER

    class ThrowingXmlResolver : XmlResolver
    {
        public static ThrowingXmlResolver Shared { get; } = new();

        public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
        {
            throw new NotSupportedException($"Unable to resolve XML entity: {absoluteUri} ({role})");
        }
    }

#endif

    static XmlReaderSettings CreateReaderSettings(NameTable nameTable) => new()
    {
        CloseInput = true,
        Async = true,
        IgnoreComments = true,
        IgnoreProcessingInstructions = true,
        IgnoreWhitespace = true,
        ConformanceLevel = ConformanceLevel.Fragment,
        DtdProcessing = DtdProcessing.Prohibit,
#if NET7_0_OR_GREATER
        XmlResolver = XmlResolver.ThrowingResolver,
#else
        XmlResolver = ThrowingXmlResolver.Shared,
#endif
        ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes,
        NameTable = nameTable
    };

    public void Reset(Stream stream)
    {
        _charStream?.Dispose();
        _reader?.Dispose();

        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);

        _charStream = new StreamReader(stream, _encoding, false, _bufferSize, true);
        _reader = XmlReader.Create(_charStream, CreateReaderSettings(_nameTable));
    }

    private Element _currentElement;

    /// <summary>
    /// Gets the element that is in the current scope being parsed.
    /// </summary>
    public Element? CurrentElement
        => _currentElement;

    /// <summary>
    /// Advances the XMPP parser.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the parser advanced successfully.
    /// <para><see langword="false" /> indicates that either.
    /// <list type="bullet">
    /// <item>the parser was disposed</item>
    /// <item>an internal error occurred</item>
    /// <item>the end of the underlying stream was reached</item>
    /// </list>
    /// </para>
    /// </returns>
    /// <exception cref="JabberStreamException">If any non-well-formed XML is detected or if the provided XML violates XMPP rules.</exception>
    public async Task<bool> ReadAsync()
    {
        if (_disposed)
            return false;

        if (_reader == null || (_reader != null && _reader.EOF))
            return false;

        bool result;
        try
        {
            result = await _reader.ReadAsync();
        }
        catch (XmlException e)
        {
            throw new JabberStreamException(StreamErrorCondition.InvalidXml, e);
        }

        if (result)
        {
            switch (_reader.NodeType)
            {
                case XmlNodeType.Element:
                    {
                        Element element;

                        if (_reader.Name != "stream:stream")
                            element = ElementFactory.Create(_reader.LocalName, _reader.Prefix, _reader.NamespaceURI);
                        else
                            element = new StreamStream();

                        if (_reader.HasAttributes)
                        {
                            while (_reader.MoveToNextAttribute())
                                element.SetAttribute(_reader.Name, _reader.Value);

                            _reader.MoveToElement();
                        }

                        if (_reader.Name == "stream:stream")
                            await OnStreamStart.InvokeAsync((StreamStream)element);
                        else
                        {
                            if (_reader.IsEmptyElement)
                            {
                                if (_currentElement != null)
                                    _currentElement.AddChild(element);
                                else
                                    await OnStreamElement.InvokeAsync(element);
                            }
                            else
                            {
                                _currentElement?.AddChild(element);
                                _currentElement = element;
                            }
                        }
                    }
                    break;

                case XmlNodeType.EndElement:
                    {
                        if (_reader.Name == "stream:stream")
                            await OnStreamEnd.InvokeAsync();
                        else
                        {
                            var parent = _currentElement.Parent;

                            if (parent == null)
                                await OnStreamElement.InvokeAsync(_currentElement);

                            _currentElement = parent;
                        }
                    }
                    break;

                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    {
                        if (_currentElement != null)
                            _currentElement.Content += _reader.Value;
                    }
                    break;

                case XmlNodeType.Whitespace:
                    // ignore
                    break;

                default:
                    if (_reader.NodeType != XmlNodeType.XmlDeclaration)
                    {
                        if (ThrownOnUnsupportedXmlFeatures)
                            throw new JabberStreamException(StreamErrorCondition.RestrictedXml);
                    }
                    break;
            }

            return true;
        }

        return false;
    }
}
