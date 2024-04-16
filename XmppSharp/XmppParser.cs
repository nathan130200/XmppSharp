using System.Text;
using System.Xml;
using System.Xml.Schema;

/* Unmerged change from project 'XmppSharp (net6.0)'
Before:
using XmppSharp.Exceptions;
After:
using XmppSharp;
using XmppSharp;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
*/
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp;

public sealed class XmppParser : IDisposable
{
    private XmlReader _reader;
    private NameTable _nameTable = new();
    private volatile bool _disposed;

    private readonly Encoding _encoding;
    private readonly int _bufferSize;

    public const int DefaultBufferSize = 256;

    public XmppParser(Encoding? encoding = default, int bufferSize = DefaultBufferSize)
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
    /// <para>Elements that cannot be constructed using <see cref="ElementFactory" /> only return element as base type of <see cref="Element" />.</para>
    /// </summary>
    public event AsyncAction<Element> OnStreamElement;

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
    /// <exception cref="ObjectDisposedException">If this instance of <see cref="XmppParser" /> has already been disposed.</exception>
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

    private Element _rootElem;

    /// <summary>
    /// Gets the XML element in current scope.
    /// </summary>
    public Element? CurrentElement
        => _rootElem;

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

    public async Task<bool> Advance()
    {
        if (_disposed)
            return false;

        if (_reader == null || _reader != null && _reader.EOF)
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
                        Element currentElem;

                        if (_reader.Name != "stream:stream")
                        {
                            var ns = _reader.NamespaceURI;

                            if (string.IsNullOrEmpty(ns) && _reader.LocalName is "iq" or "message" or "presence")
                                ns = "jabber:client";

                            currentElem = ElementFactory.Create(_reader.Name, ns);
                        }
                        else
                            currentElem = new StreamStream();

                        if (_reader.HasAttributes)
                        {
                            while (_reader.MoveToNextAttribute())
                                currentElem.SetAttribute(_reader.Name, _reader.Value);

                            _reader.MoveToElement();
                        }

                        if (_reader.Name == "stream:stream")
                        {
                            if (_reader.NamespaceURI != Namespace.Stream)
                                throw new JabberStreamException(StreamErrorCondition.InvalidNamespace);

                            await OnStreamStart.InvokeAsync((StreamStream)currentElem);
                        }
                        else
                        {
                            if (_reader.IsEmptyElement)
                            {
                                if (_rootElem != null)
                                    _rootElem.AddChild(currentElem);
                                else
                                    await OnStreamElement.InvokeAsync(currentElem);
                            }
                            else
                            {
                                _rootElem?.AddChild(currentElem);
                                _rootElem = currentElem;
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
                            if (_rootElem == null)
                                throw new JabberStreamException(StreamErrorCondition.InvalidXml, "The element in the current scope was not expected to be null.");

                            var parent = _rootElem.Parent;

                            if (parent == null)
                                await OnStreamElement.InvokeAsync(_rootElem);

                            _rootElem = parent;
                        }
                    }
                    break;

                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    {
                        if (_rootElem != null)
                        {
                            if (_rootElem.LastNode is Text text)
                                text.Value += _reader.Value;
                            else
                                _rootElem.AddChild(new Text(_reader.Value));
                        }
                    }
                    break;

                case XmlNodeType.Comment:
                    _rootElem?.AddChild(new Comment(_reader.Value));
                    break;

                case XmlNodeType.CDATA:
                    _rootElem?.AddChild(new Cdata(_reader.Value));
                    break;

                default:
                    break;
            }

            return true;
        }

        return false;
    }
}
