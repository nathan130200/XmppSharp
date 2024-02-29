using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Xml;

public sealed class XmppParser : IDisposable
{
    public event AsyncAction<Element> OnStreamStart;
    public event AsyncAction<Element> OnStreamElement;
    public event AsyncAction<Element> OnStreamEnd;

    private NameTable _nameTable = new();
    private StreamReader _baseStream;
    private XmlReader _reader;
    private volatile bool _disposed;

    public XmppParser(Stream baseStream, int bufferSize = 1024, XmlReader reader = null)
    {
        _baseStream = new StreamReader(baseStream, Encoding.UTF8, false, bufferSize, true);
        Reset();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _nameTable = null;

        _baseStream?.Dispose();
        _baseStream = null;

        _reader?.Dispose();
        _reader = null;
    }

    public void Reset()
    {
        _reader?.Dispose();

        _reader = XmlReader.Create(_baseStream, new XmlReaderSettings
        {
            CloseInput = false,
            Async = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
            ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes,
            DtdProcessing = DtdProcessing.Prohibit,
            NameTable = _nameTable,
        });
    }

    void CheckDisposed()
        => ObjectDisposedException.ThrowIf(_disposed, this);

    public async Task UpdateAsync()
    {
        CheckDisposed();

        var result = await _reader.ReadAsync();

        if (!result)
        {
            Dispose();
            return;
        }

        switch (_reader.NodeType)
        {
            case XmlNodeType.Element: await HandleStartTag(); break;
            case XmlNodeType.EndElement: await HandleEndTag(); break;

            case XmlNodeType.SignificantWhitespace:
            case XmlNodeType.Text: await HandleContent(); break;

            default:
                if (_reader.NodeType != XmlNodeType.XmlDeclaration)
                    throw new JabberException(StreamErrorCondition.InvalidXml);
                break;
        }
    }

    private Element _current;

    async Task HandleStartTag()
    {
        var el = ElementFactory.CreateElement(_reader.Name, _reader.NamespaceURI);

        if (_reader.HasAttributes)
        {
            while (_reader.MoveToNextAttribute())
                el.SetAttribute(_reader.Name, _reader.Value);

            _reader.MoveToElement();
        }

        if (el.Name == "stream:stream")
            await OnStreamStart.InvokeAsync(el);
        else
        {
            if (_reader.IsEmptyElement)
            {
                if (_current != null)
                    _current.AddChild(el);
                else
                    await OnStreamElement(el);
            }
            else
            {
                _current?.AddChild(el);
                _current = el;
            }
        }
    }

    async Task HandleEndTag()
    {
        if (_reader.Name == "stream:stream")
            await OnStreamEnd(new Element { Name = "stream:stream" });
        else
        {
            Debug.Assert(_current != null);

            if (_reader.Name != _current.Name)
                throw new JabberException(StreamErrorCondition.InvalidXml);
            else
            {
                var parent = _current.Parent;

                if (parent == null)
                    await OnStreamElement(_current);

                _current = parent;
            }
        }
    }

    Task HandleContent()
    {
        if (_current != null)
            _current.Value += _reader.Value;

        return Task.CompletedTask;
    }
}
