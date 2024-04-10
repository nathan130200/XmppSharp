using System.Text;
using System.Xml;
using System.Xml.Linq;
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
    private NameTable _nameTable = new();
    private volatile bool _disposed;

    private readonly Encoding _encoding;
    private readonly int _bufferSize;

    /// <summary>
    /// Global constant indicating the size of the initial XML buffer.
    /// </summary>
    public const int DefaultBufferSize = 256;

    /// <summary>
    /// Initializes a new <see cref="Parser"/> instance.
    /// </summary>
    /// <param name="encoding">Provides a specific encoding that will be used to read the characters. (Default <see cref="Encoding.UTF8" />)</param>
    /// <param name="bufferSize">Sets the expected size of the character buffer that will be used when reading. (Default: 64 characters)</param>
    public Parser(Encoding? encoding = default, int bufferSize = DefaultBufferSize)
    {
        _encoding = encoding ?? Encoding.UTF8;
        _bufferSize = bufferSize <= 0 ? DefaultBufferSize : bufferSize;
    }

    /// <summary>
    /// The event is triggered when the XMPP open tag is found <c>&lt;stream:stream&gt;</c>
    /// </summary>
    public event AsyncAction<StreamStream> OnStreamStart;

    /// <summary>
    /// The event is triggered when any well-formed element is found.
    /// <para>However, if the XML tag is registered using <see cref="ElementFactory" /> the parser will automatically construct the element in the registered type.</para>
    /// <para>Elements that cannot be constructed using <see cref="ElementFactory" /> only return the type <see cref="XElement" />.</para>
    /// </summary>
    public event AsyncAction<XElement> OnStreamElement;

    /// <summary>
    /// The event is triggered when the XMPP close tag is found <c>&lt;/stream:stream&gt;</c>
    /// </summary>
    public event AsyncAction OnStreamEnd;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _reader?.Dispose();
        _nameTable = null;
    }

#if !NET7_0_OR_GREATER

    internal class ThrowingResolver : XmlResolver
    {
        public static ThrowingResolver Shared { get; } = new();

        public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
        {
            throw new NotSupportedException($"Unable to resolve XML entity: {absoluteUri} ({role})");
        }
    }

#endif

    /// <summary>
    /// Restarts the internal state of the XML parser.
    /// </summary>
    /// <param name="stream">Stream that will be used to read the characters.</param>
    /// <exception cref="ObjectDisposedException">If this instance of <see cref="Parser" /> has already been disposed.</exception>
    public void Reset(Stream stream)
    {
        _reader?.Dispose();

#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
#endif

        _reader = XmlReader.Create(new StreamReader(stream, _encoding, false, _bufferSize, true), new()
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
            XmlResolver = ThrowingResolver.Shared,
#endif
            ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes,
            NameTable = _nameTable
        });
    }

    private XElement _context;

    /// <summary>
    /// Gets the XML element in current scope.
    /// </summary>
    public XElement? CurrentElement
        => _context;

    /// <summary>
    /// Gets the XML depth in the parser tree.
    /// </summary>
    public int Depth
    {
        get
        {
            if (_disposed)
                return 0;

            return _reader?.Depth ?? 0;
        }
    }

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
    public async Task<bool> Advance()
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
                        XElement self;

                        if (_reader.Name != "stream:stream")
                        {
                            var ns = _reader.NamespaceURI;

                            // WORKAROUND: Just small hack to ensure IQ, Message & Presence 
                            // will be always deserialized even if XML stream don't provide 
                            // XMLNS declaration in scope.

                            if (string.IsNullOrEmpty(ns) && _reader.LocalName is "iq" or "message" or "presence")
                                ns = "jabber:client";

                            self = ElementFactory.Create(_reader.LocalName, _reader.Prefix, ns);
                        }
                        else
                            self = new StreamStream();

                        if (_reader.HasAttributes)
                        {
                            while (_reader.MoveToNextAttribute())
                            {
                                var target = Xml.ExtractQualifiedName(_reader.Name);

                                if (!target.HasPrefix)
                                    self.SetAttribute(target.LocalName, _reader.Value);
                                else
                                {
                                    XNamespace ns = target.Prefix switch
                                    {
                                        "xml" => XNamespace.Xml,
                                        "xmlns" => XNamespace.Xmlns,
                                        _ => _reader.LookupNamespace(target.Prefix)
                                    };

                                    self.SetAttribute(ns + target.LocalName, _reader.Value);
                                }
                            }

                            _reader.MoveToElement();
                        }

                        if (_reader.Name == "stream:stream")
                            await OnStreamStart.InvokeAsync((StreamStream)self);
                        else
                        {
                            if (_reader.IsEmptyElement)
                            {
                                if (_context != null)
                                    _context.Add(self);
                                else
                                    await OnStreamElement.InvokeAsync(self);
                            }
                            else
                            {
                                _context?.Add(self);
                                _context = self;
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
                            var parent = _context.Parent;

                            if (parent == null)
                                await OnStreamElement.InvokeAsync(_context);

                            _context = parent;
                        }
                    }
                    break;

                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    {
                        if (_context != null)
                        {
                            if (_context.LastNode is XText text)
                                text.Value += _reader.Value;
                            else
                                _context.Add(new XText(_reader.Value));
                        }
                    }
                    break;

                case XmlNodeType.Comment:
                    _context?.Add(new XComment(_reader.Value));
                    break;

                case XmlNodeType.CDATA:
                    _context?.Add(new XCData(_reader.Value));
                    break;

                default:
                    break;
            }

            return true;
        }

        return false;
    }
}
