using System.Xml;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

public interface IXmppStreamProcessor
{
    bool Advance();
}

public interface IXmppChunkedParser
{
    void Write(byte[] buffer, int length);
}

public class XmppStreamReader : XmppParser, IXmppStreamProcessor
{
    private Stream? _baseStream;
    private readonly bool _leaveOpen;
    private XmlReader? _reader;
    private Element? _current;

    public XmppStreamReader(Stream baseStream, int bufferSize = -1, bool leaveOpen = true)
    {
        _baseStream = baseStream;
        _leaveOpen = leaveOpen;

        _reader = XmlReader.Create(new StreamReader(_baseStream, bufferSize: bufferSize, leaveOpen: true), new XmlReaderSettings()
        {
            CloseInput = true,
            CheckCharacters = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
#if NET6_0
            XmlResolver = Xml.ThrowingResolver
#else
            XmlResolver = XmlResolver.ThrowingResolver
#endif
        });

    }

    public virtual bool Advance()
    {
        if (isDisposed)
            return false;

        if (_reader == null)
            return false;

        if (!_reader.Read())
            return false;

        switch (_reader.NodeType)
        {
            case XmlNodeType.Element:
                {
                    var elem = ElementFactory.CreateElement(_reader.Name, _reader.NamespaceURI, _current);

                    if (_reader.HasAttributes)
                    {
                        while (_reader.MoveToNextAttribute())
                            elem.SetAttribute(_reader.Name, _reader.Value);
                    }

                    _reader.MoveToElement();

                    if (elem is StreamStream ss)
                        FireOnStreamStart(ss);
                    else
                    {
                        if (_reader.IsEmptyElement)
                        {
                            if (_current == null)
                                FireOnStreamElement(elem);
                            else
                                _current.AddChild(elem);
                        }
                        else
                        {
                            _current?.AddChild(elem);
                            _current = elem;
                        }
                    }
                }
                break;

            case XmlNodeType.EndElement:
                {
                    if (_current != null)
                    {
                        var parent = _current.Parent;

                        if (parent == null)
                            FireOnStreamElement(_current);

                        _current = parent;
                    }
                }
                break;

            case XmlNodeType.Comment:
                _current?.AddChild(new Comment(_reader.Value));
                break;

            case XmlNodeType.CDATA:
                _current?.AddChild(new Cdata(_reader.Value));
                break;

            case XmlNodeType.Text:
            case XmlNodeType.SignificantWhitespace:
                _current?.AddChild(new Text(_reader.Value));
                break;
        }

        return true;
    }

    protected override void DisposeCore()
    {
        try
        {
            _reader?.Dispose();
            _reader = null;
        }
        catch
        {
            // skip some XML errors (eg: unclosed tags)
        }

        if (!_leaveOpen)
        {
            _baseStream?.Dispose();
            _baseStream = null;
        }
    }
}
