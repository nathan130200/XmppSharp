using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using XmppSharp.Exceptions;
using XmppSharp.Protocol;

namespace XmppSharp.Xmpp;

public class XmppParser : IDisposable
{
    private XmlReader? _reader;
    private volatile bool _disposed;

    public event ParameterizedAsyncEventHandler<XmlElement> OnStreamStart;
    public event ParameterizedAsyncEventHandler<XmlElement> OnStreamElement;
    public event AsyncEventHandler OnStreamEnd;

    public XmppParser(Stream stream, Encoding? encoding = default, int bufferSize = 1024, bool leaveOpen = true)
    {
        _reader = XmlReader.Create(new StreamReader(stream, encoding ?? Encoding.UTF8, false, bufferSize, leaveOpen), new()
        {
            Async = true,
            CloseInput = true,
            IgnoreWhitespace = true,
            XmlResolver = XmlResolver.ThrowingResolver,
            IgnoreProcessingInstructions = true,
            IgnoreComments = true,
            DtdProcessing = DtdProcessing.Prohibit, // https://en.wikipedia.org/wiki/Billion_laughs_attack
        });
    }

    private XmlElement? _currentElement;

    void CheckDisposed()
        => ObjectDisposedException.ThrowIf(_disposed, this);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    int GetLine(bool isLineNum)
    {
        if (_disposed)
            return 0;

        if (_reader is not IXmlLineInfo info)
            return 0;

        if (info.HasLineInfo())
            return isLineNum ? info.LineNumber : info.LinePosition;

        return 0;
    }

    public int LineNumber => GetLine(false);
    public int LinePosition => GetLine(true);

    public async Task UpdateAsync()
    {
        CheckDisposed();

        var result = await _reader.ReadAsync();

        if (!result)
            throw new JabberStreamException(StreamErrorCondition.HostGone);

        switch (_reader.NodeType)
        {
            case XmlNodeType.Element:
                {
                    var element = Xml.Element(_reader.Name, _reader.NamespaceURI, _currentElement?.OwnerDocument);

                    if (_reader.HasAttributes)
                    {
                        while (_reader.MoveToNextAttribute())
                            element.SetAttribute(_reader.Name, _reader.Value);

                        _reader.MoveToElement();
                    }

                    if (element.Name == "stream:stream")
                        await OnStreamStart.InvokeAsync(element);
                    else
                    {
                        if (_reader.IsEmptyElement)
                        {
                            if (_currentElement != null)
                                _currentElement.AppendChild(element);
                            else
                                await OnStreamElement.InvokeAsync(element);
                        }
                        else
                        {
                            _currentElement?.AppendChild(element);
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
                        if (_reader.Name != _currentElement.Name)
                            throw new JabberStreamException(StreamErrorCondition.UnsupportedStanzaType);
                        else
                        {
                            Debug.Assert(_currentElement != null);

                            var parent = _currentElement.ParentNode as XmlElement;

                            if (parent == null)
                                await OnStreamElement.InvokeAsync(_currentElement);

                            _currentElement = parent;
                        }
                    }
                }
                break;

            case XmlNodeType.SignificantWhitespace:
            case XmlNodeType.Text:
                {
                    Debug.Assert(_currentElement != null);

                    if (_currentElement.PreviousText is XmlText text)
                        text.Value += _reader.Value;
                    else
                    {
                        text = _currentElement.OwnerDocument.CreateTextNode(_reader.Value);
                        _currentElement?.AppendChild(text);
                    }
                }
                break;

            case XmlNodeType.XmlDeclaration:
                break;

            default:
                throw new JabberStreamException(StreamErrorCondition.RestrictedXml);
        }
    }


    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _currentElement = null;
        _reader?.Dispose();
        _reader = null;

        GC.SuppressFinalize(this);
    }
}