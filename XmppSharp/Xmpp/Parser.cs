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

    protected string Name => _reader.Name;
    protected string LocalName => _reader?.LocalName;
    protected string Prefix => _reader?.Prefix;
    protected string NamespaceURI => _reader?.NamespaceURI;
    protected string Value => _reader?.Value;
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
                    await OnStartElement().ConfigureAwait(true);
                    break;

                case XmlNodeType.EndElement:
                    await OnEndElement().ConfigureAwait(true);
                    break;

                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    await OnText().ConfigureAwait(true);
                    break;

                default:
                    break;
            }

            return true;
        }

        return false;
    }

    protected Task FireStreamStart(Element e)
        => OnStreamStart.InvokeAsync(e);

    protected Task FireStreamElement(Element e)
        => OnStreamElement.InvokeAsync(e);

    protected Task FireStreamEnd()
        => OnStreamEnd.InvokeAsync();

    protected bool HasAttributes()
        => _reader.HasAttributes;

    protected bool GetNextAttribute(out (string LocalName, string Prefix) name, out string value)
    {
        name = default;
        value = default;

        if (_reader.MoveToNextAttribute())
        {
            name = (_reader.LocalName, _reader.Prefix);
            value = _reader.Value;
        }

        return false;
    }

    protected bool MoveToElement()
        => _reader.MoveToElement();

    protected bool IsEmptyElement()
        => _reader.IsEmptyElement;

    protected virtual async Task OnStartElement()
    {
        await Task.Yield();

        var element = ElementFactory.Create(LocalName, Prefix, NamespaceURI);

        if (HasAttributes())
        {
            while (GetNextAttribute(out var key, out var value))
            {
                if (!string.IsNullOrWhiteSpace(key.Prefix))
                    element.SetAttribute($"{key.Prefix}:{key.LocalName}", value);
                else
                    element.SetAttribute(key.LocalName, value);
            }

            MoveToElement();
        }

        if (Name == "stream:stream")
            await FireStreamStart(element);
        else
        {
            if (IsEmptyElement())
            {
                if (_currentElement != null)
                    _currentElement.AddChild(element);
                else
                    await FireStreamElement(element);
            }
            else
            {
                _currentElement?.AddChild(element);
                _currentElement = element;
            }
        }
    }

    protected virtual async Task OnEndElement()
    {
        if (Name == "stream:stream")
            await FireStreamEnd();
        else
        {
            var parent = _currentElement.Parent;

            if (parent == null)
                await FireStreamElement(_currentElement);

            _currentElement = parent;
        }
    }

    protected virtual async Task OnText()
    {
        await Task.Yield();

        if (_currentElement != null)
            _currentElement.Value += _reader.Value;
    }

    protected string LookupNamespace(string? prefix = default)
        => _reader?.LookupNamespace(prefix ?? string.Empty);
}
