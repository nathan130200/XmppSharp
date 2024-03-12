using System.Text;
using System.Xml;
using XmppSharp.Factory;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Xmpp;

public class Parser : IDisposable
{
    private XmlReader _reader;
    private StreamReader _charStream;
    private NameTable _nameTable = new();
    private volatile bool _disposed;

    private readonly Encoding _encoding;
    private readonly int _bufferSize;

    protected NameTable NameTable => _nameTable;

    public const int DefaultBufferSize = 64;

    public Parser(Encoding? encoding = default, int bufferSize = -1)
    {
        _encoding = encoding;
        _bufferSize = bufferSize <= 0 ? DefaultBufferSize : bufferSize;
    }

    public event AsyncAction<Protocol.Base.Stream> OnStreamStart;
    public event AsyncAction<Element> OnStreamElement;
    public event AsyncAction OnStreamEnd;

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _charStream?.Dispose();
        _reader?.Dispose();
        _nameTable = null;

        GC.SuppressFinalize(this);
    }

    protected XmlReaderSettings CreateReaderSettings() => new()
    {
        CloseInput = true,
        Async = true,
        IgnoreComments = true,
        IgnoreProcessingInstructions = true,
        IgnoreWhitespace = true,
        ConformanceLevel = ConformanceLevel.Fragment,
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = XmlResolver.ThrowingResolver,
        ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.AllowXmlAttributes,
        NameTable = _nameTable
    };

    public void Reset(Stream stream)
    {
        _charStream?.Dispose();
        _reader?.Dispose();

        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);

        _charStream = new StreamReader(stream, _encoding, false, _bufferSize, true);
        _reader = XmlReader.Create(_charStream, CreateReaderSettings());
    }

    private Element _currentElement;

    protected Element? CurrentElement
    {
        get => _currentElement;
        set => _currentElement = value;
    }

    public virtual async Task<bool> ReadAsync()
    {
        if (_disposed)
            return false;

        if (_reader == null || (_reader != null && _reader.EOF))
            return false;

        var success = await _reader.ReadAsync();

        if (success)
        {
            switch (_reader.NodeType)
            {
                case XmlNodeType.Element:
                    {
                        var element = ElementFactory.Create(_reader.LocalName, _reader.Prefix, _reader.NamespaceURI);

                        if (_reader.HasAttributes)
                        {
                            while (_reader.MoveToNextAttribute())
                                element.SetAttribute(_reader.Name, _reader.Value);

                            _reader.MoveToElement();
                        }

                        if (_reader.Name == "stream:stream")
                            await OnStreamStart.InvokeAsync((Protocol.Base.Stream)element);
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
                            _currentElement.Value += _reader.Value;
                    }
                    break;

                default:
                    break;
            }

            return true;
        }

        return false;
    }
}
