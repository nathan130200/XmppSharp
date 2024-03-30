using System.Text;
using System.Xml;
using XmppSharp.Factory;

namespace XmppSharp.Dom;

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

    public event AsyncAction<Element> OnStreamStart;
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

#if !NET7_0_OR_GREATER

    class ThrowingXmlResolver : XmlResolver
    {
        public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
        {
            throw new InvalidOperationException($"Unable to resolve XML entity: {absoluteUri} ({role})");
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
        XmlResolver = new ThrowingXmlResolver(),
#endif
        ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.AllowXmlAttributes,
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
                            await OnStreamStart.InvokeAsync(element);
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
